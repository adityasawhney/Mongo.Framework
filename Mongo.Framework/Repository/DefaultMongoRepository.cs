using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Mongo.Framework.Exception;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Mongo.Framework.Repository
{
	/// <summary>
	/// Implementation of MongoRepository interface using 10gen C# driver
	/// </summary>
	/// <typeparam name="T">The entity type</typeparam>
	public class DefaultMongoRepository<T> : IMongoRepository<T> where T : class
	{
		private readonly MongoCollection<T> collection;

		public DefaultMongoRepository(MongoCollection<T> collection)
		{
			this.collection = collection;

			// If the type isnt registered then invoke the static ctor of DAO to register type mapping
			if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
			{
				RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
			}
		}

		public T Add(T entity)
		{
			try
			{
				this.collection.Insert<T>(entity);

			}
			catch (MongoException e)
			{
				// Check if exception was thrown for duplicate key: E11000
				//
				// ISSUE: 
				//	No clean way to handle this but an enhancement is logged
				//	https://jira.mongodb.org/browse/SERVER-3069
				//
				if (e.Message.Contains("E11000") || e.Message.ToLower().Contains("duplicate key"))
				{
					throw new AlreadyExistsException(string.Format("Mongo Entity '{0}'", entity));
				}
				throw;
			}
			return entity;
		}

		public void Add(IEnumerable<T> entities)
		{
			this.collection.InsertBatch<T>(entities);
		}

		public T Single(Expression<Func<T, bool>> criteria)
		{
			return this.collection.AsQueryable().Where(criteria).FirstOrDefault();
		}

		public IQueryable<T> All()
		{
			return this.collection.AsQueryable();
		}

		public IQueryable<T> All(int page, int pageSize)
		{
			return Page(All(), page, pageSize);
		}

		public IQueryable<T> All(Expression<Func<T, bool>> criteria)
		{
			return this.collection.AsQueryable().Where(criteria);
		}

		public MongoCursor<T> FindAs(IMongoQuery query)
		{
			return this.collection.FindAs<T>(query);
		}

		public T Update(T entity)
		{
			this.collection.Save<T>(entity);
			return entity;
		}

		public void Update(IEnumerable<T> entities)
		{
			foreach (T entity in entities)
			{
				this.collection.Save<T>(entity);
			}
		}

		public bool Update(IMongoQuery query, IMongoUpdate update, UpdateFlags flags)
		{
			return this.collection.Update(query, update, flags).DocumentsAffected > 0;
		}

		public T FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool returnNew, bool upsert)
		{
			return this.collection.FindAndModify(query, sortBy, update, returnNew, upsert).GetModifiedDocumentAs<T>();
		}

		public T FindAndRemove(IMongoQuery query, IMongoSortBy sortBy)
		{
			return this.collection.FindAndRemove(query, sortBy).GetModifiedDocumentAs<T>();
		}

		public bool Delete(T entity)
		{
			object idValue = BsonClassMap.LookupClassMap(typeof(T)).IdMemberMap.Getter(entity);
			return Delete(Query.EQ("_id", BsonValue.Create(idValue))) > 0;
		}

		public long Delete(IMongoQuery query)
		{
			return this.collection.Remove(query).DocumentsAffected;
		}

		public void RemoveAll()
		{
			this.collection.RemoveAll();
		}

		private static IQueryable<T> Page(IQueryable<T> source, int page, int pageSize)
		{
			return source.Skip((page - 1) * pageSize).Take(pageSize);
		}
	}
}

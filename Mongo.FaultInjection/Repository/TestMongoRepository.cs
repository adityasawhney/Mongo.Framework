using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Test.FaultInjection;
using Mongo.Framework.Repository;
using MongoDB.Driver;

namespace Mongo.FaultInjection.Repository
{
	/// <summary>
	/// MongoRepository for test purposes which triggers faults based on rule configuration.
	/// This is a decorator over the regular mongo repository implementation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class TestMongoRepository<T> : IMongoRepository<T> where T : class, new()
	{
		private readonly IMongoRepository<T> repository;

		public event FaultHandler FaultListeners;

		public TestMongoRepository(IMongoRepository<T> repository)
		{
			this.repository = repository;
		}

		public T Add(T entity)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				return HandleFaultEvent<T>(MongoRepositoryHelper.Method.ADD, a, b);
			}

			return this.repository.Add(entity);
		}

		public void Add(IEnumerable<T> entities)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
			}

			this.repository.Add(entities);
		}

		public T Single(Expression<Func<T, bool>> criteria)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				return HandleFaultEvent<T>(MongoRepositoryHelper.Method.SINGLE, a, b);
			}

			return this.repository.Single(criteria);
		}

		public IQueryable<T> All()
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return new EnumerableQuery<T>((IEnumerable<T>)b);
			}

			return this.repository.All();
		}

		public IQueryable<T> All(int page, int pageSize)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return new EnumerableQuery<T>((IEnumerable<T>)b);
			}

			return this.repository.All(page, pageSize);
		}

		public IQueryable<T> All(Expression<Func<T, bool>> criteria)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return new EnumerableQuery<T>((IEnumerable<T>)b);
			}

			return this.repository.All(criteria);
		}

		public MongoCursor<T> FindAs(IMongoQuery query)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (b != null) b = new TestMongoCursor<T>((IEnumerable<T>)b);
				return HandleFaultEvent<MongoCursor<T>>(MongoRepositoryHelper.Method.FIND_AS, a, b);
			}

			return this.repository.FindAs(query);
		}

		public T Update(T entity)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return (T)b;
			}

			return this.repository.Update(entity);
		}

		public void Update(IEnumerable<T> entities)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
			}

			this.repository.Update(entities);
		}

		public bool Update(IMongoQuery query, IMongoUpdate update, UpdateFlags flags)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				return HandleFaultEvent<bool>(MongoRepositoryHelper.Method.UPDATE_CAS, a, b);
			}

			return this.repository.Update(query, update, flags);
		}

		public T FindAndModify(IMongoQuery query, IMongoSortBy sortBy, IMongoUpdate update, bool returnNew, bool upsert)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return (T)b;
			}

			return this.repository.FindAndModify(query, sortBy, update, returnNew, upsert);
		}

		public T FindAndRemove(IMongoQuery query, IMongoSortBy sortBy)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return (T)b;
			}

			return this.repository.FindAndRemove(query, sortBy);
		}

		public bool Delete(T entity)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return (bool)b;
			}

			return this.repository.Delete(entity);
		}

		public long Delete(IMongoQuery query)
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
				return (long)b;
			}

			return this.repository.Delete(query);
		}

		public void RemoveAll()
		{
			Exception a;
			object b;
			if (FaultDispatcher.Trap(out a, out b))
			{
				if (a != null) throw a;
			}

			this.repository.RemoveAll();
		}

		private TReturn HandleFaultEvent<TReturn>(string methodName, Exception exceptionValue, object returnValue)
		{
			if (FaultListeners != null)
			{
				var faultEvent = new FaultEvent()
				{
					MethodName = methodName,
					Exception = exceptionValue,
					ReturnValue = returnValue
				};
				FaultListeners(faultEvent);
			}
			if (exceptionValue != null) throw exceptionValue;
			return (TReturn)returnValue;
		}

		class TestMongoCursor<TClass> : MongoCursor<TClass> where TClass : class, new()
		{
			private readonly IEnumerable<TClass> documents;

			public TestMongoCursor(IEnumerable<TClass> documents)
				: base(null, null, ReadPreference.Primary, null, null)
			{
				this.documents = documents;
			}

			public override IEnumerator<TClass> GetEnumerator()
			{
				return this.documents.GetEnumerator();
			}
		}
	}
}

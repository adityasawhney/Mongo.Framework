Mongo.Framework
===============

Mongo framework for .Net which builds on top the 10gen's official Mongo C# driver by providing standard DAL patterns and constructs that enable rapid application development.

The framework provides following capabilities:
- Constructs for Data Access Layer (DAL) abstraction
- Repository pattern abstraction to interface with Mongo driver
- Standardized pattern to define Object Model, its ORM and versioning scheme
- Transient fault handling and configurable retry logic using the Enterprise Library
- Command pattern to provide stored procedure like semantics
- Constructs to enable ACID semantics using Compare And Swap(CAS) approach
- Constructs to generate various hashes (MD5, Murmur2)
- Constructs to generate identifiers similar to Identity column in SQL
- Fault injection framework to test ACID semantics of commands

For complete description of the design of the framework refer to following wiki:  
http://lotusinmud.wordpress.com/2013/07/13/mongo-client-framework-for-net/

For illustration on how to use the framework refer to the **Mongo.Framework.Example** project.

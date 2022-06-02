---
title: DP-203 Study Guide part 2 - Cosmos DB
abstract: Part 2 of the Azure Data Engineering Associate study guide focusing on Cosmos DB
date: 2022-05-16
tags: DP203, Cosmos DB 
---
# Implementing non-relational data stores

The [DP-203: Azure Data Engineering Associate](https://docs.microsoft.com/en-us/learn/certifications/azure-data-engineer/) exam requires subject matter expertise in Azure solutions for non-relational data storage. This section of the syllabus includes:

- Azure Storage
- Cosmos DB
- Azure Data Lake

This part 2 guide will cover [Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction). 

## Introduction to Cosmos DB
Building a highly scalable, globally distributed database is hard. 
In 2010, Microsoft realised they needed to build something very different to SQL server to handle global distribution needs.

In 2015, Microsoft released Azure Document DB, a distributed database that supported SQL queries over JSON documents.

In 2017, Azure Document DB was rebranded as Cosmos DB.

## Features
- Cloud based NoSQL database / Database as a service (DaaS)
- Turnkey globally distributed
- Serverless architecture
- No operational overhead
- No schema or index management (=> no application downtime while schema is migrated)
- Multi-model (Key value, Json documents, graph, and table/columnar data store)
- Multi-language APIs (Java, .NET, Python, Node.js, JavaScript etc.) 
- Highly available, reliable and secure (always on, 99.999% uptime, < 10ms latency)
- Unlimited scale for both storage and throughput.
- Supports configurable consistency levels: strong, eventually, consistent prefix, session and bounded-staleness.

## Use cases
- Highly distributed
- Highly available
- IoT
- Retail and marketing
- Gaming
- Web and mobile applications

## Multi-model support
Cosmos DB supports data storage in a number of model. Each model has its own API
- Key-value -> Table API
- Wide-column (table/columnar) -> Cassandra API
- Graph -> Gremlin API
- Document -> SQL(Core) API and MongoDB API

### Document APIs
The SQL(Core) API is the original Document DB platform API and support storage of Json documents. 
The SQL API is also the only API that supports a SQL-like query language and a server-side programming model for transactional stored procedures.

`Exam tip: any question references a SQL-like query language then SQL(Core) API will be the answer.`   

MongoDB API is an API that is fully compatible with MongoDB (implementing the same wire protocol and supporting storage of documents in BSON format).
This API allows migration of existing MongoDB solutions to Cosmos DB with minimal changes to the connection string.

Microsoft advise that the SQL(Core) API should be used for new developments.

### Cosmos DB Table API
The Cosmos DB Table API provides a key-value store API that supported the same protocol as Azure Table Storage API. It can be viewed as a premium offering for this API.
Existing customers using Azure Table Storage can migrate to Cosmos DB using the Table API.
Row values can be simple value like a string or number.
Rows cannot store objects/documents. 

### Cosmos DB Cassandra API
Cassandra is a wide column NoSQL database. The Cosmos DB Cassandra API supports the same wire protocol as Cassandra allowing simple migrations from
Cassandra to Cosmos DB and the use of Cassandra tools such as Data Explorer and SDKs such as the Cassandra CSharp Driver.

### Cosmos DB Gremlin API
The Cosmos DB Gremlin API supports the Gremlin Graph data model. Again, the Gremlin wire protocol is supported (allowing easy migration from Gremlin to Cosmos DB).
Efficient for graph traversal (e.g. friends in social networks, device connections in IoT solution, etc.)

## Provisioning a Cosmos DB Account
When provisioning a Cosmos DB account you need to select the storage model/API that the Cosmos DB will support. Only one storage model is supported per account and
this value cannot be changed. Properties such as multi-region support and multi-master writes can be configured during account creation and can be modified later.

An account must have a unique name as this is used as part of the public URL to access the account (e.g. https://[Cosmos_DB_Account_Name].documents.azure.com).

The account is the top level component of data organisation.
- The Account contains Databases (also known as Keyspaces in the Cassandra API).
- Databases/Keyspaces contain Containers which are realised as Collections (SQL API and MongoDB API), Tables (Cassandra API and Table API), or Graphs (Gremlin API)
- Containers contain Items which are realised as Documents (SQL API and MongoDB API), Rows (Cassandra API), Items (Table API), or Nodes and Edges (Gremlin API) 
- Containers can also contain stored procedures, user-defined functions, triggers, conflicts, and merge procedures (note this is *not* at the database level like with traditional SQL server)

### Creating a database container

Accounts using provisioned throughput can contain up to [500 databases](https://docs.microsoft.com/en-us/azure/cosmos-db/concepts-limits) and 500 containers (although only 25 containers per database can use shared throughput).

Accounts using serverless throughput can contain up to 500 containers.

## Measuring Performance
There are two main aspects to database performance: latency and throughput
- Latency: how fast is the response for a given request? To lower latency we can make sure service is close to the user.
- Throughput: how many requests can be served within a specific period of time? Cosmos DB is designed to handle many workloads. The maximum throughput can be increased by specifying more **Request units** (for an increased cost).

### Throughput
Request Units (RU/s) are used to measure throughput. RUs represent a combination of memory, CPU and IOPs. RUs can be measured using the Data Explorer (see the Query Stats tab in the Results).

RUs can be provisioned at the database (shared) or dedicated for a particular container. The minimum number of RUs that can be provisioned is 400. The default maximum is 100,000 RUs (but this limit can be increased and is, theoretically, unlimited).
400 RUs is roughly $0.58 per hour.

If reserved throughput limits are exceeded than requests are throttled 
(APIs return a `429 - Too Many Requests` error). Multiple 429 errors indicate the provisioned limit should be increased.

It is recommended to set the throughput at the container level.

## Horizontal Scaling
Cosmos DB can store an unlimited amount of data.

A single CosmosDB container may be implemented by multiple physical machine (depending on the data storage requirements and provisioned RU throughput).

## Partitioning
Each container has a partition key. The partition key determines how the container divides data amongst the underlying physical machines. All data with a given partition key becomes part of the same logical partition (data is stored together). Multiple logical partitions may be stored on the same underlying physical machines. However this is internal to CosmosDB and is not something which we have control over.

- Partitioning: the items in a container are divided into distinct subsets called logical partitions.
- Partition key is the value by which Azure organises your data into logical divisions.
- Logical partitions are formed based on the value of a partition key this is associated with each item in a container.
- Physical partitions: internally, one or more logical partitions are mapped to a single physical partition. A single logical partition **cannot** be divided into multiple physical partitions. Logical partitions can, over time, be moved from one physical partition to another - this migration is completely transparent.

### Avoiding hot partitions
Container RUs are divided between the logical partitions. RUs allocated to one partition cannot be used by other partitions. This means that if one partition is very active then it may reach the RU threshold and cause `429 - Too Many Requests` error even if RUs are available in other partitions.

To ensure logical partitions are used effectively, a partition key that evenly distributes data amongst partitions should be used If one partition has the majority of the data then it will consume more RUs and may be rate limited - this is a 'hot' partition on store. 

Similarly, we should try to ensure all queries can be distributed evenly across all available partitions. A query that just queries one partition can result in that partition consuming all available RUs and being rate limited - this is a 'hot' partition on throughput. Using a date as a partition key and having a system where most users are querying on the current day could cause a hot partition on throughput.

### Single partition and cross partition queries

A single partition query is one where CosmosDB can infer (from the partition key of the container) that data matching the query being performed can be retrieved from a single partition. This is efficient.

A cross partition (or fan out) query requires CosmosDB to run a query across all partitions and combine the results. These should be avoided where possible.

### Composite keys
CosmosDB has some limits with regards to data:
- A single document cannot exceed 2MB of data.
- A single partition cannot exceed 20GB of data.











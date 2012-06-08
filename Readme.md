#Umbraco Courier Open libraries	

###What is Courier?
Courier is a closed-source deployment tool for the Umbraco CMS. It helps you understand dependencies and resources used by each individual item you deploy and handles changing IDs in the database. All handled from within the Umbraco UI.

###How to use the source?
The source code, does not include the source of the Courier core, but it does contain all the providers that collects data, figures out dependencies and collects resources. 

So the source is  reference for those who wish to build their own provider on top of the Courier engine, the Courier core, currently has these extension points:

* Repository types
* Item providers
* Data resolvers
* Resource resolvers
* Event handlers
* Cache handlers

All these providers are in a ongoing documentation process, the status of which you can follow below.

## Documentation
Documentation is currently being updated and put into markdown instead of those awful PDFs, current documents available:

* Dataresolvers
* Item event handlers
* Event Queues
* Xml depency libraries

In process:

* Packaging engine API
* Extraction engine API

## Pull requests
Yes, we accept pull requests, the source represented here, is the current source tip in the courier development repository. Its handled seperately to devide core and provider libraries.



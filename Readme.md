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
* Xml dependency libraries

In process:

* Packaging engine API
* Extraction engine API

## Pull requests
Yes, we accept pull requests, the source represented here, is the current source tip in the courier development repository. Its handled seperately to devide core and provider libraries.


##Current Code samples and projects

###Damp Resolver
Sample Propertyresolver for Digibiz advanced mediapicker

###Courier Teamdev
Code for deplying multiple revisions, and sample setup for nant and teamcity

###Umbraco.Courier.SampleResolver
Item data resolver

###Umbraco.Courier.SubversionRepository
Sample subversion repository to submit revisions directly to SVN from courier

###Umbraco.Courier.DataResolvers
The built-in dataresolvers, included in the core of Courier 2

###Umbraco.Courier.ExtractionConsole
Sample Console application for doing packaging and extraction from the commandline

###Umbraco.Courier.Ucomponents
Library for resolving uComponents datatypes


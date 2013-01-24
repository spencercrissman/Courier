#Nant Tasks for Umbraco Courier Deployments

Courier.Tasks is a standard nant plugin, which allows you to call courier packaging and extraction directly from your nant build 
scripts.

It uses the default Courier manifest files for setting what items are packaged, and the standard courier.config file for loading
repository data.



##Packaging

Simply add a package task to your build, with the following configuration options:

- config: path to your courier.config files
- source: the alias of the source repository
- target: the alias of the potential destination (optional)
- revision: name of the revision - this creates a folder at /app_data/courier/revisions/
- manifest: path to the manifest xml, which defines what providers and items are queried


Code:

	<package config="${courier.config}" source="cws" target="clean" revision="infrastructure" manifest="${manifests.dir}\infrastructure.xml" />

Sample manifest:

	<?xml version="1.0" ?>
	<manifest>
		<metaData>
			<description></description>
			<uniqueIdentifier>00000000-0000-0000-0000-000000000000</uniqueIdentifier>
			<createDate>2013-01-24T11:58:57.5866345+01:00</createDate>
			<updateDate>2013-01-24T11:58:57.5866345+01:00</updateDate>
		</metaData>
	<providers>

	<!-- content -->
	<provider id="d8e6ad83-e73a-11df-9492-0800200c9a66" includeAll="True" dependecyLevel="0"/>

	<!-- media -->
	<provider id="d8e6ad87-e73a-11df-9492-0800200c9a66" includeAll="True" dependecyLevel="0" />
	</providers>
	</manifest>

However, it is much easier to define the contents of the revision in the courier section in umbraco and
simply reuse the generated manifest file, which will be located in the revision folder in /app_data/courier/revisions/name


##Extraction
Same process for extracting a previosly packaged revision

- config: path to your courier.config files
- target: the alias of the potential destination (optional)
- revision: name of the revision - this queries a folder at /app_data/courier/revisions/ for files

   <extract config="${courier.config}" target="clean" revision="infrastructure" />

##Installation
Add the following dll's to c:\program files\nant\bin\ 

- Courier.Tasks.dll
- Umbraco.Courier.Core.dll (from courier dist)
- Umbraco.Licensing.dll (from courier dist)
- businesslogic.dll (from umbraco dist)
- interfaces.dll (from umbraco dist)

To add any courier plugins, you will need to add them to c:\program files\nant\bin\courierplugins. By default
you should have

- Umbraco.Courier.Providers.dll
- Umbraco.Courier.RepositoryProviders.dll


##Sample project and nant build files

Can be found in the /sampleproject folder. To use, simply run nant in that folder and it will run any .build file, which will trigger Courier






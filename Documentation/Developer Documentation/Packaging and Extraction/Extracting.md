##Extracting a revision

If you already have data converted into a revision, you can use the RevisionExtraction to import it into another site

###Load the revision
This by default sets the source and destination to whatever local context it can find.

	RevisionExtraction engine = new RevisionExtraction("revisionName");

when you set the source and destination, courier will pull files and data FROM the source
but it will also extract items and files TO the destination site
so you can run the code outside of any umbraco context and trigger transfer between 2 external sources

	RepositoryStorage rs = new RepositoryStorage();
	var source = rs.GetByAlias("local");
	var destination = rs.GetByAlias("remote");
	rs.Dispose();

	var compareEngine = new RevisionExtraction(source, destination, "RevisionName");

###Run the extraction

	engine.Extract();

###Run as task
If you want to run this in a web context, but in a background thread, you can hook into couriers task manager, by running extractions and packaging as tasks

	Umbraco.Courier.Core.Tasks.ExtractionTask et = new ExtractionTask();
	et.RevisionAlias = "revision";
	et.SourceRepository = "local";
	et.DestinationRepository = "remote";
            
add to the task manager, which will then execute when theres a thread available in background
	
	Umbraco.Courier.Core.TaskManager.Instance.Add(et);

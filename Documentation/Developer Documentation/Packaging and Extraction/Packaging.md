this tells courier to store the packaging result to revisionName folder at the local site

	RevisionPackaging engine = new RevisionPackaging("RevisionName");
            
there are some overloads, which allows you to connect to different locations
these are stored in courier.config

!!NOTICE
when you set the source and destination, courier will pull files and data FROM the source
but it will also store the revision items and files AT the destination site

	RepositoryStorage rs = new RepositoryStorage();
	var source = rs.GetByAlias("local");
	var destination = rs.GetByAlias("remote");
	var compareEngine = new RevisionPackaging(source, destination, "RevisionName");

you can also tell courier to compare the contents of the packaging with a running site, so you dont package
items that already exists at the target location
  
	compareEngine.EnableInstantCompare(destination);
            
we can then add some items to the queue for packaging

this always relies on getting a uniqueu ID for each type of item, so documents use GUIDs, doctypes use alias' and so on
for each Item ID, we also have to specify which provider are supposed to handle it
            
	engine.AddToQueue(
		new ItemIdentifier("DocumentGuid", ProviderIDCollection.documentItemProviderGuid));

	engine.AddToQueue(
		new ItemIdentifier("textpage", ProviderIDCollection.documentTypeItemProviderGuid));

	engine.AddToQueue(
		new ItemIdentifier("macroAlias", ProviderIDCollection.macroItemProviderGuid));

alternatively, you can query each provider about what items they have;

	var docProvider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(ProviderIDCollection.documentItemProviderGuid);
	var rootItems = docProvider.AvailableSystemItems();
	engine.AddToQueue(rootItems, true, -1);

or drill into each item

	if (rootItems[0].HasChildren)
	{
		var childItems = docProvider.AvailableSystemItems(rootItems[0].ItemId);
	}

courier uses a manifest format which is stored in each revision folder, you can also use this to load items
and have a predefined manifest for each transfer

	var manifest = RevisionManifest.Load(@"c:\manifest.xml");
	engine.AddToQueue(manifest);

when items have been added, we can package the queue, this will compile the selection of items into a dependency tree
so we ensure all needed depencies are included in the package

	engine.Package();

if you want feedback from courier, you can subscribe to packaging events

	engine.PackagedItem += engine_PackagedItem;
	engine.PackagedItemResource += engine_PackagedItemResource;
#Providers

Courier comes with a collection of different providers and provider types. This document outlines the types, and each individual provider, and what it does. 

##Dataresolvers
A dataresolver is a way to add meaning to objects courier doesn't understand. For instance, if you have a document with a custem datatype, and the datatype stores a NodeID reference to another page (like a content picker), Courier doesn't know the number refers to another node, but by adding a dataresolver, you can tell courier that items with the datatype, contains a node ID, and add the needed dependencies and resources to have a succesfull deployment.

###Included data resolvers
####AscxFiles
* **Full name:** `Umbraco.Courier.DataResolvers.ascxFiles`
* **Triggers on:** Macros which have a .ascx file as its macro file
* Collects resources for ascx files on macros, this can both be .dll or .cs files

####ContentPicker
* **Full name:** `Umbraco.Courier.DataResolvers.ContentPicker`
* **Triggers on:** Propertydata, which have a contentpicker as datatype (configured in courier.config)
* If value is set, and is an int, courier will convert the value to the Node GUID, add the node as a dependency. On extraction the GUID will be converted back to the right ID.

####CSSResources
* **Full name:** `Umbraco.Courier.DataResolvers.CssResources`
* **Triggers on:**Stylesheets
* Includes images included in the stylesheet as resources.

####DampResolver
* **Full name:** `Umbraco.Courier.DataResolvers.DampResolver`
* **Triggers on:** Propertydata, which have a DAMP pick as datatype
* If value is set, and is an int, courier will convert the value to the media GUID, add the media item as a dependency. On extraction the GUID will be converted back to the right ID.

####EmbeddedContent
* **Full name:** `Umbraco.Courier.DataResolvers.EmbeddedContent`
* **Triggers on:** Propertydata, which have a EmbeddedContent type as datatype 
* Replaces node IDs in the embedded content with corresponding GUIDs and converts them back again on extraction

####Images
* **Full name:** `Umbraco.Courier.DataResolvers.Images`
* **Triggers on:** Propertydata, which contains a RTE 
* Finds linked images in the RTE html and sorts out IDs paths, and resources.


####KeyValuePrevalueEditor
* **Full name:** `Umbraco.Courier.DataResolvers.KeyValuePrevalueEditor`
* **Triggers on:** Propertydata, which contains a keyvalue editor like dropdownlist, radiobutton list, checkboxlist 
* Resolves prevalues from IDs to actual value, and back again on extraction.

####LocalLinks
* **Full name:** `Umbraco.Courier.DataResolvers.LocalLinks`
* **Triggers on:** Propertydata, which contains a the string {locallink: 
* Resolves the ID to a guid, and adds the linked document as a dependency

####MacroParameters
* **Full name:** `Umbraco.Courier.DataResolvers.MacroParameters`
* **Triggers on:** Propertydata and Templates, which contains `<umbraco:macro/>` elements
* Looks at each property and checks if it contains a node ID reference. If it does, the reference is changed to a Guid, and the node is added as a dependency.

####MediaPicker
* **Full name:** `Umbraco.Courier.DataResolvers.MediaPicker`
* **Triggers on:** Propertydata, which have a mediapicker as datatype (configured in courier.config)
* If value is set, and is an int, courier will convert the value to the media GUID, add the node as a dependency. On extraction the GUID will be converted back to the right ID.

####RelatedLinks
* **Full name:** `Umbraco.Courier.DataResolvers.RelatedLinks`
* **Triggers on:** Propertydata, which have a RelatedLinks type as datatype
* If values are set, courier will convert the values to the corresponding GUIDs, add the nodes as dependencies. On extraction the GUIDs will be converted back to the right IDs.


####RTEstylesheets
* **Full name:** `Umbraco.Courier.DataResolvers.RTEstylesheets`
* **Triggers on:** The Rich text editor DataType
* If the RTE have any stylesheets associated, these will be added as dependencies to the datatype

####Tags
* **Full name:** `Umbraco.Courier.DataResolvers.Tags`
* **Triggers on:**  Propertydata, which have a Tags type as datatype
* Selected tags are included as seperate dependencies and extracted along with the document.

####TemplateResources
* **Full name:** `Umbraco.Courier.DataResolvers.TemplateResources`
* **Triggers on:**  Templates
* Detects linked images, javascript files and stylesheets. These are added as resources and dependencies on packaging. It also detects locallinks in the template and adds the linked Node as a dependency.

####UltimatePicker
* **Full name:** `Umbraco.Courier.DataResolvers.UltimatePicker`
* **Triggers on:**  Propertydata, which have a UltimatePicker type as datatype
* Selected node IDs are converted to GUIDs and the linked nodes are added as dependencies


####Upload
* **Full name:** `Umbraco.Courier.DataResolvers.Upload`
* **Triggers on:**  Propertydata, which have a Upload type as datatype
* If upload field contains a file, the file is added as a resource on the document.

####UsercontrolWrapper
* **Full name:** `Umbraco.Courier.DataResolvers.UsercontrolWrapper`
* **Triggers on:**  The UsercontrolWrapper Datatypes
*If the datatype has a .ascx file selected as render, the file is added as a resource to ensure it is trasfered with the datatype.

##ResourceResolvers
A resource resolver is slightly different from data resololvers, as it only triggers when a resource/file is packaged and extracted. This means it can only modify the file itself, but not call back to the item, which the resource belongs to

####MacroParameters
* **Full name:** `Umbraco.Courier.ResourceResolvers.MacroParameters`
* **Triggers on:**  Files with the extension .master and contains umbraco:macro elements.
* Modifies the template file itself to convert any Node Ids in any macro to a guid, and vice versa.

####TemplateResources
* **Full name:** `Umbraco.Courier.ResourceResolvers.TemplateResources`
* **Triggers on:**  Files with the extension .master
* Modifies the template file itself and replaces any locallink references with a GUID





















#Provider configuration

_Courier comes with a number of built-in providers to package and extract data, some of these have different confiuration settings, which can be found below_


###Files
Configuration can expose certain folders and files in the item picker in the courier section use `file` and `folder` elements
in the configuration to add specific paths, wildcards are not accepted:

  &lt;fileItemProvider&gt;
    &lt;folder&gt;~/media/assets/somefolder&lt;/folder&gt;
    &lt;file&gt;~/media/assets/somefile.png&lt;/file&gt;
  &lt;/fileItemProvider&gt;

###Folders
Same as above, but only with folders

  &lt;folderItemProvider&gt;
      &lt;folder&gt;~/media/assets/somefolder&lt;/folder&gt;
  &lt;/folderItemProvider&gt;

###Mediatypes

- Single option to turn of whether allowed/child media types should be added as a dependency or not

  &lt;mediaTypeItemProvider&gt;
    &lt;includeChildMediaTypes&gt;true&lt;/includeChildMediaTypes&gt;
  &lt;/mediaTypeItemProvider&gt;
  
###DocumentTypes

- Option to include all allowed templates as a dependency
- option to include all allowed types as a dependency, 
- option filter out certain datatypes:

  &lt;documentTypeItemProvider&gt;
      &lt;!-- Include all avaiable templates as dependencies, if false, only the current standard template is included --&gt;
      &lt;includeAllTemplates&gt;false&lt;/includeAllTemplates&gt;
      &lt;includeChildDocumentTypes&gt;true&lt;/includeChildDocumentTypes&gt;
      
      &lt;!-- By default we won't add the built-in datatypes as dependencies, if needed, they can be removed from the list below --&gt;
      &lt;!-- Only datatypes which are installed as standard, and does not have any settings are ignored --&gt;
      &lt;!-- to add, find the datatype in the umbracoNode table and copy its uniqueId value to a node below--&gt;
      &lt;ignoredDataTypes&gt;
        &lt;add key=&quot;contentPicker&quot;&gt;A6857C73-D6E9-480C-B6E6-F15F6AD11125&lt;/add&gt;
        &lt;add key=&quot;textstring&quot;&gt;0CC0EBA1-9960-42C9-BF9B-60E150B429AE&lt;/add&gt;
        &lt;add key=&quot;textboxmultiple&quot;&gt;C6BAC0DD-4AB9-45B1-8E30-E4B619EE5DA3&lt;/add&gt;
        &lt;add key=&quot;label&quot;&gt;F0BC4BFB-B499-40D6-BA86-058885A5178C&lt;/add&gt;
        &lt;add key=&quot;folderbrowser&quot;&gt;FD9F1447-6C61-4A7C-9595-5AA39147D318&lt;/add&gt;
        &lt;add key=&quot;memberpicker&quot;&gt;2B24165F-9782-4AA3-B459-1DE4A4D21F60&lt;/add&gt;
        &lt;add key=&quot;simpleeditor&quot;&gt;1251C96C-185C-4E9B-93F4-B48205573CBD&lt;/add&gt;
        &lt;add key=&quot;truefalse&quot;&gt;92897BC6-A5F3-4FFE-AE27-F2E7E33DDA49&lt;/add&gt;
        &lt;add key=&quot;contentpicker&quot;&gt;A6857C73-D6E9-480C-B6E6-F15F6AD11125&lt;/add&gt;
        &lt;add key=&quot;datepicker&quot;&gt;5046194E-4237-453C-A547-15DB3A07C4E1&lt;/add&gt;
        &lt;add key=&quot;datepickerWithTime&quot;&gt;E4D66C0F-B935-4200-81F0-025F7256B89A&lt;/add&gt;
        &lt;add key=&quot;numeric&quot;&gt;2E6D3631-066E-44B8-AEC4-96F09099B2B5&lt;/add&gt;
      &lt;/ignoredDataTypes&gt;
    &lt;/documentTypeItemProvider&gt;
    
###Media

- Option to include parent nodes as a forced dependency 
- option to automaticly include all children (in case a folder is transfered) 

  &lt;mediaItemProvider&gt;
      &lt;includeChildren&gt;false&lt;/includeChildren&gt;
      &lt;includeParents&gt;false&lt;/includeParents&gt;
    &lt;/mediaItemProvider&gt;

###Documents

- Option to include parents as dependencies

   &lt;documentItemProvider&gt;
      &lt;includeParents&gt;true&lt;/includeParents&gt;
    &lt;/documentItemProvider&gt;
    
###Templates

- Option to collect macros found in templates as a dependecy, 
- toggle if courier should look for files linked in the template(js,css,image files)
- Collect locallink: references and add the documents as dependencies
- Parse macro's and add any nodeIds passed to the macro as a dependency

  &lt;templateItemProvider&gt;
    &lt;macrosAreDependencies&gt;true&lt;/macrosAreDependencies&gt;
    &lt;processTemplateResources&gt;true&lt;/processTemplateResources&gt;
    &lt;localLinksAreDependencies&gt;true&lt;/localLinksAreDependencies&gt;
    &lt;macroParametersAreDependencies&gt;true&lt;/macroParametersAreDependencies&gt;
  &lt;/templateItemProvider&gt;

###Ignore
You can ignore providers during app start by passing in their name:
  
  &lt;ignore&gt;
    &lt;add&gt;TemplateItemProvder&lt;/add&gt;
  &lt;/ignore&gt;

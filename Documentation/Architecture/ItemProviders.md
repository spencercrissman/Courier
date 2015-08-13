#Itemproviders

_Itemproviders included in Courier.Providers by default and their guids_


##Datatypes
Contains a single datatype, can contain dependencies to dlls, picked content/media, 
uses guid as key

  e0472592-e73b-11df-9492-0800200c9a66

##Dictionary
Contains a single dictionary item, can contain a dependency on its parent dictionary item, 
uses key as key

  d8e6ad86-e73a-11df-9492-0800200c9a66

##Document
Contains a single node without property data, can contain dependencies to document types, parent node, 
uses guid as key

  d8e6ad83-e73a-11df-9492-0800200c9a66
  
##DocumentType
Single document type, contains dependencies on its used datatypes, allowed templates, composite document types, dictionary items used in labels, 
uses alias as key

  d8e6ad84-e73a-11df-9492-0800200c9a66");

##File
Single file as a dependence, rarely used, 
uses path as key

  2ab3b250-e28d-11df-85ca-0800200c9a66

##Folder
Single folder as a dedendency, adds all files in folder as a file dependency, 
uses path as key

  d8e6ad80-e73a-11df-9492-0800200c9a66
  
##Language
Language, contains no dependencies, 
uses its iso-code as key

  d8e6ad81-e73a-11df-9492-0800200c9a66

##MacroGuiRendering
Parameter editor dependency for macros, not used in Version 7.

  d8e6ad89-e73a-11df-9492-0800200c9a66

#Macro
Macro, contains no dependencies, but does depend on the property editors used, 
uses alias as key

  2ab40e30-e292-11df-85ca-0800200c9a66


##Media
Contains a single node without property data, can contain dependencies to media types, parent node, 
uses guid as key

  d8e6ad87-e73a-11df-9492-0800200c9a66
  
##MediaType
Single media type, contains dependencies on its used datatypes, composite media types, dictionary items used in labels, 
uses alias as key

  d8e6ad88-e73a-11df-9492-0800200c9a66
  

##MemberGroup
  4715aa16-fa35-426f-bb67-674043557875
  
##MemberType
Single member type, contains dependencies on its used datatypes, dictionary items used in labels, 
uses alias as key
  9bbce930-5deb-4775-bbc6-4e4e94dfa0db

##Document PropertyData
Contains all values from a single nodes properties, its dependencies depends on the types of property editors used and are collected by
resolvers for those editors - ex: a content picker, will add the picked node as a dependency, 
uses node guid as key

  e0472594-e73b-11df-9492-0800200c9a66

##Media PropertyData
Contains all values from a single nodes properties, its dependencies depends on the types of property editors used and are collected by
resolvers for those editors - ex: a content picker, will add the picked node as a dependency, 
uses node guid as key

  e047259a-e73b-11df-9492-0800200c9a66
  
##Relation
Contains all relations for a single node, has dependencies to the relation types used, 
uses relation parent guid as key

  d8e6ad82-e73a-11df-9492-0800200c9a66
  
##RelationType
A single relation type, has no dependencies, 
Uses alias as key

  d8e6ad90-e73a-11df-9492-0800200c9a66

##Stylesheet
Stylesheet, in pre-7.3 it contains a list of stylesheet properties and the stylesheet file it uses, 
Uses name as key

  e0472596-e73b-11df-9492-0800200c9a66

##StylesheetProperty 
Not in use anymore, but was used to contain a single css property, with a dependency to the stylesheet using it.
  32534278-5AA3-4975-91ED-ED748420177D

##TagRelations
Contains all tags for a single node, only used for cotnent and media, contains a dependency on the node using the tags.
Uses node guid as key.

  e0472599-e73b-11df-9492-0800200c9a66

##Template
Template, contains dependncy on parent template, for masterpages it was detected if the html contained macro and node Id references, but not for cshtml views.
uses alias as key.
  25867200-e67e-11df-9492-0800200c9a66

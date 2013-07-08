#Common issues

_Document to outline commonly found issues with courier, and possible solutions_

###Key not found exception
*Caused by:* Courier not being able to find a speicific provider, commonly the datalayer provider. Usually because Courier
didnt load the datalater dll or one of its dependencies. 

*How to spot:* Enable debugging in /config/courier.config, restart the app and check the 
/app_data/courier/logs/error_log.txt file for exceptions releated to loading providers. Usually it will say which dll 
had issues loading. 

If no exceptions in the log, it might be missing the dll, or have the dll for the wrong version.

Courier for V6 expects to have umbraco.courier.persistence.v6.nhibernate.dll and for V4 it should have 
umbraco.courier.persistence.v4.nhibernate.dll

####Courier cannot package items on V4
*Caused by:* Missing dlls or dlls from the wrong courier version

*How to spot:* On v6 sites, it has dlls from V4 distribution, on V4 sites it has V6 dlls, or are missing dlls. To debug,
put courier in debug mode in /config/courier.config and restart application, it should throw exceptions to 
/app_data/courier/logs/log_error.txt about missing dependencies or wrong versions

*Solution:* Uninstall courier, and reinstall 2.7.8.14 from nightly:
http://nightly.umbraco.org/UmbracoCourier/2.7.8/nightly%20builds/



*Solution:* Ensure that all dlls are loaded properly, and that it has all the dlls expected. Also, for it should only have
the proper dlls for the specific versions. 

Get the dlls from 2.7.8.14 here: 
http://nightly.umbraco.org/UmbracoCourier/2.7.8/nightly%20builds/

And copy to /bin

####Content are deployed but not published
*Caused by:* Exception during extraction which might get the publish event not to fire

*How to spot:* Content transfers with no error, but changes are not visble on the website

*Solution:* upgrade to 2.7.8.14 which has a bug fix
http://nightly.umbraco.org/UmbracoCourier/2.7.8/nightly%20builds/

####Tabs dont inherite on Umbraco 6
*Caused by:* Changes in the V6 datalayer in the way tabs are inherited from parent document types

*How to spot:* Inherited tabs are created as new tabs on the document type

*Solution:* Upgrade to 2.7.8.14
http://nightly.umbraco.org/UmbracoCourier/2.7.8/nightly%20builds/

####Files on NAS are not transfered
*Caused by:* file paths break the way courier looks up files and cannot find them when transfering

*How to spot:* Files are not included in the revisions

*Solution:* Upgrade to 2.7.8.14







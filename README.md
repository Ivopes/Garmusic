# Garmusic
bachelor's thesis

## Prerequisites
* Node.js
* .NET 5 SDK
* Visual Studio 2019
* SQL Server and SSMS

# How To setup
Guide on how to setup the app to run locally.  
Please note that some features (storage webhooks) are not available when runned locally.  
You can also check the app working [here](http://garmusic.azurewebsites.net/)

## Get the repository
1. Clone the repo 
```
git clone https://github.com/Ivopes/Garmusic.git
cd Garmusic\ClientApp
```
2. Install npm packages
```sh
npm install
```

## Create the database
1. Open SSMS
2. Run CreateDatabase.sql script from Garmusic\Scripts
3. Run InsertData.sql script Garmusic\Scripts

## Create a Dropbox app
1. Visit the [Dropbox App Console](https://www.dropbox.com/developers/apps)
2. Create an Dropbox app
  * Select App folder as Scoped access
3. Add https://localhost:44303/dbx to your app's redirect URIs
4. Set the Access token expiration to No expiration
5. Check files.content.write and files.content.read permissions in the permissions tab
6. Copy your App key and secret and replace them in appsettings.Development.json
```csharp
"DropboxKey": "ENTER YOUR APP KEY",
"DropboxSecret": "ENTER YOUR APP SECRET"
```

## Create a Google Drive app
1. Visit the [Google Guide](https://developers.google.com/workspace/guides/create-project)
2. Create a Google app by the instructions
3. Enable a Google Drive API to you app as described [here](https://developers.google.com/drive/api/v3/enable-drive-api)
4. Create a Credentials as described [here](https://developers.google.com/workspace/guides/create-project).
You may need to verify the angular app by adding metatag to index.html
5. Add https://localhost:44303 to Authorized JavaScript origins.  
Add folowing strings to Authorized redirect URIs:  
https://127.0.0.1/gd,  
https://localhost/gd,  
https://localhost/authorize,  
https://127.0.0.1/authorize,  
http://localhost/authorize/,  
http://127.0.0.1/authorize/

6. Download the credentials and replace the content of the googleDriveSecrets.json with them 

## Set connection strings
1. Replace your database connection string in appsettings.Development.json
```csharp
"MusicPlayerConnection": "ENTER YOUR CONNECTION STRING"
```

## How to run
1. Open Garmusic.sln in Visual Studio 2019
2. Build and run the application (F5)
3. Open https://localhost:44303 in Google Chrome browser

## TL;DR
If you want to save yourself a precious time then go and check the app already working [here](http://garmusic.azurewebsites.net/)

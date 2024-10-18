


# Getting Started

The Local Environment set up is the same as before.

I hope you find some enjoyment out of the work I did!

Looking forward to talking with you soon,

-Matt Norman

# Feature Improvements

## Main Feature Improvements

### 1. Real-time Notifications & Unit Tests
Notifications are seen on the notification dropdown in the NavBar. 
I implemented a simple unit test and a simple integration test for just adding a song. These can be seen in the Tests project. 

### 2. File Upload
This can be done when adding a new song, or by clicking the plus icon when expanding a song on the home page. 

## Smaller Improvements

### 1. Search Functionality
You can now search in the upper right corner, with four choices of filters. 
I opted for a full refresh of the page to save time. Due to the size of the database, I feel eventually this would be
better placed as a client size filtering.

### 2. Error Handling and Conventions
There are now multiple try-catch statements in the Controller and program.cs. As for conventions, 
I turned the App_Data folder into AppData, as well as turning the music-manager-starter.Data folder into Data. 

### 3. Logging
Serilog is implemented in most server-side functions. Logs are flushed to console, as well as a daily log in /Logs.

### 4. Song Details
Song Details are seen when a song's accordion is expanded. 

## Other Improvements
The main UI improvement is changing the display blocks of the songs to an accordion stack. I really love this set up, as it lets the user have more choice about levels of information.




## Technologies
- Visual Studio 2022 
- .NET 8 SDK
- Node.js (for Tailwind CSS)
- Git
- EntityFramework Core 
- Blazor


## Local Enviroment Setup
You need to have the .NET 8 SDK installed. Be sure to download the latest version for Visual Studio 2022.

Use the latest version of Visual Studio 2022: https://visualstudio.microsoft.com/downloads/

Install the node packages before building the solution by using ```npm install``



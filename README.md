<h1>Set up development environment</h1>

<ul><h3>Step 0. Install GIT</h3>
    <li>Link for installation git: <a href="https://www.atlassian.com/git/tutorials/install-git">here</a></li>
</ul>
<ul><h3>Step 1. Install MongoDB <strong>v. 3.6.12</strong> and Compass (UI for using MongoDB)</h3>
    <li>Link for installation MongoDB and Compass: <a href="https://docs.mongodb.com/v3.6/tutorial/install-mongodb-on-windows/">here</a></li>
    <li><p>Add MongoDB to global PATH variable: </p>
        <p>This PC => Properties => Advanced system settings => Environment Variables</p>
        <p>System variables => Select => <strong>Path</strong> Textfield => Click <strong>Edit...</strong></p>
        <p>Click <strong>New</strong></p>
        <p>Paste this path: C:\Program Files\MongoDB\Server\3.6\bin</p>
    </li>
</ul>

<ul>
    <h3>Step 2. Create Base Mongo DB</h3>
    <li>Create C:\data folder</li>
    <li>Create C:\data\db folder</li>
    <li>Run in cmd: <strong>"C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe" --dbpath="c:\data\db"</strong></li>
    <i>NOTE: Now MongoDB server starts, for stop it close cmd window (without started db Mongo will not working)</i>
</ul>

<ul>
    <h3>Step 3. Restore DB from dump</h3>
    <li>Run command <strong>mongorestore --db inqury (*PATH TO DEPUTIES PROJECT*)/Deputies.Database.Dump/inqury</strong></li>
</ul>

<ul>
    <h3>Step 4. Run Project</h3>
    <li>Open Visual Studio and run project</li>
</ul>

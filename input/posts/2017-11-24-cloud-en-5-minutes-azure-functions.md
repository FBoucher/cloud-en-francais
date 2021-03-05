---
title: Azure Functions et Visual Studio Code
permalink: /cloud-en-5-minutes-azure-functions
Published: 2017-11-24
tags: [cloud, infonuagique, azure, visualstudio, ubuntu, function, clouden5, code, vscode]
---

Dans cette capsule de Cloud en 5 minutes, Je partage une découverte récente qui parmet de développer et déployer des Azure Functions à partir de Linux, Mac et WIndows, and utilisant Visual Studio Code.

<div class="container ">
<iframe class="youtubevideo" width="560"  src="https://www.youtube.com/embed/ilCpq29_vaE" frameborder="0" allowfullscreen></iframe>
</div>

Voici le code pour la function et le fichier pour installer `tedious`.

##### TimerTriggerJS.js

```javascript

var Connection = require('tedious').Connection;
var Request = require('tedious').Request
var TYPES = require('tedious').TYPES;

module.exports = function (context, myTimer) {

    var _currentData = {};

    var config = {
        userName: 'frankadmin',
        password: 'MyPassw0rd!',
        server: 'clouden5srv.database.windows.net',
        options: {encrypt: true, database: 'clouden5db'}
    };

    var connection = new Connection(config);
    connection.on('connect', function(err) {
        context.log("Connected");
        getPerformance();
    });

    function getRandom(min, max) {
        return Math.random() * (max - min) + min;
    }

    function getPerformance() {

        request = new Request("SELECT 'Best' = MIN(FivekmTime), 'Average' = AVG(FivekmTime) FROM RunnerPerformance;", function(err) {
        if (err) {
            context.log(err);}
        });

        request.on('row', function(columns) {
            _currentData.Best = columns[0].value;
            _currentData.Average = columns[1].value;;
            context.log(_currentData);
        });

        request.on('requestCompleted', function () {
            saveStatistic();
        });
        connection.execSql(request);
    }


    function saveStatistic() {

        request = new Request("UPDATE Statistic SET BestTime=@best, AverageTime=@average;", function(err) {
         if (err) {
            context.log(err);}
        });
        request.addParameter('best', TYPES.Int, _currentData.Best);
        request.addParameter('average', TYPES.Int, _currentData.Average);
        request.on('row', function(columns) {
            columns.forEach(function(column) {
              if (column.value === null) {
                context.log('NULL');
              } else {
                context.log("Statistic Updated.");
              }
            });
        });

        connection.execSql(request);
    }

    context.done();
};

```



##### package.json
```json

{
  "name": "tedious",
  "version": "2.1.1",
  "description": "Connect to Database",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1"
  },
  "repository": {
    "type": "git",
    "url": "git+https://github.com/tediousjs/tedious.git"
  },
  "author": "",
  "license": "ISC",
  "bugs": {
    "url": "https://github.com/tediousjs/tedious/issues"
  },
  "homepage": "https://github.com/tediousjs/tedious#readme"
}

```



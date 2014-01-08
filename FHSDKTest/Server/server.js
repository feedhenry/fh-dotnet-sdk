var express = require("express");

var app = express();


var os = require('os');
var ifaces = os.networkInterfaces();
var ipaddress = '127.0.0.1';

var os = require('os');
var ifaces = os.networkInterfaces();
ifaces["Ethernet"].forEach(function (details) {
    if (details.family == 'IPv4') {
        ipaddress = details.address;
    }
});

console.log("IPAddres", ipaddress);

var port = 3001;

var initResponse = {
	"domain":"testing",
	"firstTime":false,
	"hosts": {
		"debugCloudType":"node",
		"debugCloudUrl":"http://" + ipaddress + ":" + port,
		"releaseCloudType":"node",
		"releaseCloudUrl":"http://" + ipaddress + ":" + port
	},
	"init":{"trackId":"oP4zdn1ltddXstCvvkVKL-MI"},
	"status":"ok"};

app.use(express.bodyParser({}));

app.post('/app/init', function(req, res){
	res.send(JSON.stringify(initResponse));
});

app.post('/cloud/echo', function(req, res){
	var data = req.body;
	console.log("data", data);
	var resdata = {'request': data};
	res.send(JSON.stringify(resdata));
});

app.listen(port);
var jsonData_JAN;
var jsonParsed_JAN;
var value_JAN;

var jsonData_APR;
var jsonParsed_APR;
var value_APR;



google.charts.load('current', { 'packages': ['corechart'] });
// https://stackoverflow.com/questions/47502800/passing-in-parameter-in-google-charts-function


google.charts.setOnLoadCallback(function () {
    // alert("test 1");
    drawChart(7, 'NO2', 'µg/m³',  30, 'chart_div_S7-NO2');
    drawChart(7, 'PM10', 'µg/m³', 40, 'chart_div_S7-PM10');
});

// https://www.umweltbundesamt.at/grenzwerte/ 

/*
  <div id="das-ist-drei">
    <h3>Station Drei Name</h3>
    <div  style="width: 100%; display:inline-block;">
      <span id="chart_div_S3-NO2"  style="display:inline-block; width: 48%; height: 500px;"></span>
      <span id="chart_div_S3-PM10" style="display:inline-block; width: 48%; height: 500px;"></span>
    </div>
  </div>
*/

function drawChart(stationID, 
                   airParamKey, airParamUnit, airParamMax,
                   chartDivID) {

    //////////////////////////////////////////////////////////////////
    // VIEW Optionen-Setup
    // 
    // Chart-Setup (verschiedene Optionen)    
    var options = {

        title:
            'Station:  ' + stationID + ', ' +
            'Messwert: ' + airParamKey + ' [' + airParamUnit + ']',
        legend: 'top',

        hAxis: {
            title: 'Monatstag',
            titleTextStyle: {
                color: '#333',
                fontSize: 15,
            },
            textStyle: {
                fontSize: 10
            },
        },
        vAxis: {
            title: airParamKey + ' [' + airParamUnit + ']',
            titleTextStyle: {
                color: '#333',
                fontSize: 15,
            },
            textStyle: {
                fontSize: 10
            },
            minValue: 0
        },
        // Einzelne Daten-Serien ("Areas")
        series: {
            0: { // Optionen für Area: (JAN)
                color: '#f36daa',  // "lila" 
                areaOpacity: 0.3   // 0.0 = transparent, 1.0 = fully opaque, https://developers.google.com/chart/interactive/docs/gallery/areachart           
            },
            1: { // Optionen für Area:  (APR)
                color: '#3fc26b', // "grün"
                areaOpacity: 0.3
            },
            2: { // Optionen für  (max)-Linie
                color: '#f44336', // "rot"
                areaOpacity: 0.0
            }
        }
    };

    //////////////////////////////////////////////////////////////////
    // JAN & APR Daten laden
    // 
    // URL-Beispiel
    //    "https://localhost:5001/odata/Measurements?$filter=(MeasurementDate%20gt%202020-04-01T00:00:00-00:00%20and%20MeasurementDate%20lt%202020-04-30T00:00:00-00:00)%20and%20StationId%20eq%20'7'&$select=measurementdate,NO,NOX,stationid"     
    // 
    var url_JAN =
        "https://localhost:5001/odata/Measurements?$filter=("+
        "MeasurementDate%20gt%202020-01-01T00:00:00-00:00%20and%20MeasurementDate%20lt%202020-01-30T00:00:00-00:00)"+
        "%20and%20StationId%20eq%20"+      "'" + stationID +  "'"  +
        "&$select="        +
        "measurementdate"  + "," +
        airParamKey;       
    var url_APR =
        "https://localhost:5001/odata/Measurements?$filter=("+
        "MeasurementDate%20gt%202020-04-01T00:00:00-00:00%20and%20MeasurementDate%20lt%202020-04-30T00:00:00-00:00)"+
        "%20and%20StationId%20eq%20"+      "'" + stationID +  "'"  +
        "&$select="        +
        "measurementdate"  + "," +
        airParamKey;       

           
    jsonData_JAN = $.ajax({
        url: url_JAN,
        dataType: "json",
        async: false
    }).responseText;
    jsonData_APR = $.ajax({
        url: url_APR,
        dataType: "json",
        async: false
    }).responseText;
    //
    jsonParsed_JAN = JSON.parse(jsonData_JAN);
    jsonParsed_APR = JSON.parse(jsonData_APR);
    // get JS objects out of JSON Strings
    value_JAN = jsonParsed_JAN["value"];
    value_APR = jsonParsed_APR["value"];

    //////////////////////////////////////////////////////////////////
    //  MODELL:  Numerische Daten: Parsing
    //
    var data = new google.visualization.DataTable();
    // Voraussetzungen: 
    // -  JAN & APR Daten: gleiche Felder im JSON 
    // -  erstes Feld im Set: MeasurementDate
    // -  alle Daten sind nummerische Werte 
    //    (MeasurementDate wird ins "Tag der Monat" umgewandelt)
    // - beide JSONs haben gleicher Anzahl von "value"-Objekten (lenght of array gleich)
    //
    var indexMeasurementDate = 0;
    var type = 'number';  // 'string', 'number', 'boolean', 'date', 'datetime', and 'timeofday'.
                          // Mehr über Column: https://developers.google.com/chart/interactive/docs/reference#DataTable_addColumn
    data.addColumn(type, 'Tag');
    data.addColumn(type, 'JAN');
    data.addColumn(type, 'APR');
    data.addColumn(type, 'Max: ' + airParamMax + ' ' + airParamUnit );

    // numerische Daten   
    for (var i = 0; i < value_JAN.length; i++) {
        // get values
        var paramValue_JAN = value_JAN[i][airParamKey];
        var paramValue_APR = value_APR[i][airParamKey];
        // aus "2020-04-02T00:00:00+02:00" => Datum-Objekt
        var mesDateTime_JAN = new Date(value_JAN[i]['MeasurementDate']);
        // MeasurementDate string (if existing) should be changed to datetime Object to get date of month 
        var valueArray = [
            mesDateTime_JAN.getDate(),  // 0 .. 31
            paramValue_JAN,
            paramValue_APR, 
            airParamMax,
        ];
        data.addRow(valueArray);

    }

/*
"value":[
    {"MeasurementDate":"2020-04-02T00:00:00+02:00","NO":6 },
 */
    //////////////////////////////////////////////////////////////////
    // CONTROLLER: Modell und View an die Google Charts übergeben
    // 
    var chart = new google.visualization.AreaChart(
        document.getElementById(chartDivID));
    chart.draw(data, options);
}

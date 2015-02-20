function requestAll(oData, callback, req) {
    console.log(JSON.stringify(oData));
    $.ajax({
        url: 'http://localhost:17875/Service.aspx',
        // this is the parameter list
        data: oData,
        type: 'POST',
        success: function (output) {
            // parse the data here
            try {
                output = JSON.parse(output);
            }
            catch (err) {
                output = {
                    "error": "JSON parse error",
                    "errorccode": "1"
                }
            }
            callback(output, req);
        },
        error: function () {
        }
    });

}
(function () {
    "use strict";
    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.

            var searchButton = document.getElementById("SearchButton");
            searchButton.addEventListener("click", this.searchHandler, false);
            // this will be after the list is generated for every a tag
        },
        searchHandler: function (eventInfo) {
            var searchText = document.getElementById("SearchBox").value;
            requestAll({
                "searchText": searchText,
                "requestType": 1
            }, searchResult, "");

            // Save the session data. 
         //   WinJS.Application.sessionState.greetingOutput = greetingString;
        }
      
    });

    function linkClickEventHandler (eventInfo) {
        eventInfo.preventDefault();
        var link = eventInfo.target;
        var data = {};
        data["PaperLink"] = $(link).attr("data-PaperLink");
        data["PaperTitle"] = $(link).attr("data-PaperTitle");
        data["PaperID"] = $(link).attr("data-PaperID");
        WinJS.Navigation.navigate(link.href,data);
    }
    
    
    function searchResult(result, req) {

        WinJS.Utilities.removeClass(document.getElementById("PaperWrapper"), "Hidden");
        $("#PaperWrapper").html("");
        if (!result.length) {
            $("#PaperWrapper").text("No matching papers found.")

        }
        for (var i = 0; i < result.length; i++) {
            var div = $("<div>");
            var link = $("<a>");
            link.attr("href", "/pages/paper/paper.html");
            link.attr("data-PaperLink", result[i]["PaperLink"]);
            link.attr("data-PaperID", result[i]["PaperID"]);
            link.attr("data-PaperTitle", result[i]["PaperTitle"]);
            link.text(result[i]["PaperTitle"]);
            link.addClass("pList");
            div.append(link);
            $("#PaperWrapper").append(div);
            WinJS.Utilities.query("#PaperWrapper a").listen("click",
                linkClickEventHandler, false);
        }
    }
})();

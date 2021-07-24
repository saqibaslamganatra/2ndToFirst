// Online Demo Toolbar

$(function () {
    $("#online-demo-toolbar #toolbar-tab a").click(function (e) {
        e.preventDefault();
        toggleODT();
    });

    // get the value from the "odt" cookie
    var cookieValue = $.cookie("odt");
    //alert(cookieValue);

    // default to open state
    var targetClass = "open"; // "closed";

    // if the value is closed, use that value
    if (cookieValue == "c") {
        targetClass = "closed";
    }

    var tabOpenContainerMargin = "130px";
    var tabClosedContainerMargin = "0px";
    var thisUrl = new String(document.location).toLowerCase();

    if (thisUrl.indexOf("/admin") > 0) {
        tabOpenContainerMargin = "145px";
        tabClosedContainerMargin = "0px;"; //"25px";
    }

    // Set the toolbar class to value determined above
    $("#online-demo-toolbar").attr("class", targetClass);

    if ($("#online-demo-toolbar").hasClass("open")) {
        $("#Container").css("margin-top", tabOpenContainerMargin);
        $("#online-demo-toolbar .content").show();
    } else {
        $("#Container").css("margin-top", tabClosedContainerMargin);
    }
});

function toggleODT() {

    // animate content open or closed
    $("#online-demo-toolbar .content").animate({
        "height": "toggle"
    }, 500);

    var tabOpenContainerMargin = "130px";
    var tabClosedContainerMargin = "0px";
    var thisUrl = new String(document.location).toLowerCase();
    if (thisUrl.indexOf("/admin") > 0) {
        tabOpenContainerMargin = "145px";
        tabClosedContainerMargin = "0px;"; //"25px";
    }

    // If open before, must be closed now
    if ($("#online-demo-toolbar").hasClass("open")) {
        $("#online-demo-toolbar").attr("class", "closed");
        // store the closed value in a cookie for next visit, up to 30 days
        $.cookie('odt', "c", { path: '/', expires: 30 });
        $("#Container").css("margin-top", tabClosedContainerMargin);
    }

    // If closed before, must be open now
    else {
        $("#online-demo-toolbar").attr("class", "open");
        $("#Container").css("margin-top", tabOpenContainerMargin);
        $.cookie('odt', "o", { path: '/', expires: 30 });
    }
}

 jQuery.support.cors = true;
 var salesAddress = "/api/sales";

 $(function () {
     var SalesHeader = salesAddress + "/" + getUrlVars()["SalesHeader"];
     $.getJSON(
        SalesHeader,
        function (value) {
            $("#SalesDetails").html("");
            value.OrderDate = value.OrderDate.substring(0, 10);
            $("#SalesTemplate").tmpl(value).appendTo("#SalesDetails");
            $("#ProductTemplate").tmpl(value.products).appendTo("#ProductsDetails");
        }
    );

        $("#btnShopAgain").live("click", function () {
            window.location.replace("Products.htm");
        });
 });


function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}


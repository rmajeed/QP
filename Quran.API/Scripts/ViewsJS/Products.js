jQuery.support.cors = true;
var productcategoriesAddress = "/api/products/";

$(function () {
    $('#tblRight').hide();

    $.getJSON(
            productcategoriesAddress,
            function (data) {
                var parents = jQuery.grep(data, function (a) { return a.ParentProductCategoryID == null });
                var childs = jQuery.grep(data, function (a) { return a.ParentProductCategoryID != null });
                $.each(parents,
                    function (index, value) {
                        var categorydata = [];
                        var subCategory = '';
                        var subChild = jQuery.grep(childs, function (a) { return a.ParentProductCategoryID == value.ProductCategoryID });
                        $.each(subChild,
                            function (index, item) {
                                var serviceURL = productcategoriesAddress + item.ProductCategoryID;
                                subCategory = subCategory + "  " + "<a href=" + serviceURL + " class='menuButton'>" + item.Name + "</a>";
                            });
                        categorydata.push({
                            'ProductCategoryID': value.ProductCategoryID,
                            'ParentCategory': value.Name,
                            'ChildCategory': subCategory
                        });
                        $("#categoryTemplate").tmpl(categorydata).appendTo("#categories");
                        $("#" + value.Name).html(subCategory);
                    });

                    GetProducts(1);
            }
        );

    $("#Checkout").click(function () {
        $("#divMain").hide();
    });

    $(".menuButton").live("click", function () {
        $('#categories li h1').css('background', '#736F6E');
        $("#loader").show();
        $.ajax({
            type: "GET",
            datatype: "json",
            url: $(this).attr("href"),
            context: this,
            success: function (value) {
                $("#productData").html("");
                $("#productTemplate").tmpl(value).appendTo("#productData");
                $("#loader").hide();
            }
        });
        return false;
    });

    $(".AddToCart").live("click", function () {
        var selectedItem = $.tmplItem(this);
        var productQuantity = $("#txt" + selectedItem.data.ProductID).val();
        if (productQuantity == "" || productQuantity == null) {
            alert("Please select quantity for this product.")
            $("#txt" + selectedItem.data.ProductID).focus();
            return false;
        }
        else {
            if ($('#tblRight').hide())
                $('#tblRight').show();
            var totalPrice = parseFloat($("#lblCartTotal").html()) + (parseFloat(selectedItem.data.ListPrice) * parseFloat(productQuantity));
            $("#lblCartTotal").html(totalPrice.toFixed(2));
            var cartData = [];
            cartData.push({
                'ProductID': selectedItem.data.ProductID,
                'ProductCode': selectedItem.data.ProductNumber,
                'Quantity': productQuantity,
                'Price': (parseFloat(selectedItem.data.ListPrice) * parseFloat(productQuantity)).toFixed(2)
            });
            $("#CartProductTemplate").tmpl(cartData).appendTo("#cartData");
            $("#txt" + selectedItem.data.ProductID).val('');
        }
        return false;
    });

    $(".removeFromCart").live("click", function () {
        var currentPrice = parseFloat($(this).closest("tr").children()[2].innerHTML);
        $(this).closest("tr").remove();
        var totalPrice = parseFloat($("#lblCartTotal").html());
        $("#lblCartTotal").html(parseFloat(totalPrice - currentPrice).toFixed(2));
        if ($('#cartData').children().length == 0)
            $('#tblRight').hide();
    });

    $("body").addClass("ui-widget");
    $("input[type=\"submit\"], .menuButton").button()

});

function ChecOutItem() {
    var cartDataArray = new Array();

    $('#cartData tr').each(function () {
        var obj = this.cells[0].innerHTML + '|' + this.cells[2].innerHTML;
        cartDataArray.push(obj);
    });

    window.location.replace("checkout.htm?cart=" + cartDataArray);
}

function GetProducts(ProductCategoryID) {
    var serviceURL = productcategoriesAddress + ProductCategoryID;
    $('#categories li h1').css('background', '#736F6E');
    $('#mnu' + ProductCategoryID).css('background', '#357EC7');
    $("#loader").show();
    $.ajax({
        type: "GET",
        datatype: "json",
        url: serviceURL,
        context: this,
        success: function (value) {
            $("#productData").html("");
            $("#productTemplate").tmpl(value).appendTo("#productData");
            $("#loader").hide();
        }
    });
    return false;
}
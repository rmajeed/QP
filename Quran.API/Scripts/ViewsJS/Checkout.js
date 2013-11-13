 jQuery.support.cors = true;
 var customersAddress = "/api/customers";
 var salesAddress = "/api/sales";
var URL = "http://localhost:30000/api/";

$(function () {
    jQuery("#customerInformation").validationEngine();
    var cartDetails = getUrlVars()["cart"].replace('#', '');
    $(".Login").live("click", function () {
        var userName = $("#txtUserID").val();
        var password = $("#pwdPassword").val();
        if (userName == "" || userName == null) {
            alert("Customer ID cannot be null.")
            $("#txttxtUserID").focus();
            return false;
        }
        else if (password == "" || password == null) {
            alert("Password cannot be null.")
            $("#pwdPassword").focus();
            return false;
        }
        else {
            var parameter = "/ValidateCustomer/" + userName + "/" + password;
            $.ajax({
                type: "GET",
                dataType: "json",
                url: customersAddress + parameter,
                success: function (value) {
                    if (value == null) {
                        alert("Record not found with provided credentials, please register first.");
                    }
                    else {
                        $("#customerInformation").show();
                        $("#tblLogin").hide();
                        document.getElementById("lblName").innerHTML = value.FirstName + " " + value.LastName;
                        $("#tblLogout").show();
                        $("#txtCustomerID").val(value.CustomerID);
                        $("#txtTitle").val(value.Title);
                        $("#txtFirstName").val(value.FirstName);
                        $("#txtMiddleName").val(value.MiddleName);
                        $("#txtLastName").val(value.LastName);
                        $("#txtSuffix").val(value.Suffix);
                        $("#txtCompanyName").val(value.CompanyName);
                        $("#txtEmailAddress").val(value.EmailAddress);
                        $("#txtPhone").val(value.Phone);
                        $("#txtPassword").val(password);
                        $("#txtConfirmPassword").val(password);

                        $("#txtBAddressLine1").val(value.BillingAddressLine1);
                        $("#txtBAddressLine2").val(value.BillingAddressLine2);
                        $("#txtBCity").val(value.BillingCity);
                        $("#txtBStateProvince").val(value.BillingStateProvince);
                        $("#txtBCountryRegion").val(value.BillingCountryRegion);
                        $("#txtBPostalCode").val(value.BillingPostalCode);

                        $("#txtSAddressLine1").val(value.ShippingAddressLine1);
                        $("#txtSAddressLine2").val(value.ShippingAddressLine2);
                        $("#txtSCity").val(value.ShippingCity);
                        $("#txtSStateProvince").val(value.ShippingStateProvince);
                        $("#txtSCountryRegion").val(value.ShippingCountryRegion);
                        $("#txtSPostalCode").val(value.ShippingPostalCode);
                        $("#btnReset").hide();
                        $("#btnRegister").val("Update");
                        $("#btnsubmitOrder").show();
                    }
                }
            });
        }
        return false;
    });

    $("#btnsubmitOrder").live("click", function () {
        var object = {
             ProductWithQuantity : cartDetails,
             CustomerID : $("#txtCustomerID").val()
         };

        $.post(
            salesAddress,
            object,
            function (value) {
                window.location.replace("Confirmation.htm?SalesHeader=" + value);
            },
            "json"
        );
    });

    $("#customerInformation").submit(function () {
        if (jQuery('#customerInformation').validationEngine('validate')) {
            $.post(
                    customersAddress,
                    $("#customerInformation").serialize(),
                    function (value) {
                        if ($("#txtCustomerID").val() == 0) {
                            alert("Record added successfully.");
                            $("#txtCustomerID").val(value);
                            $("#btnRegister").val("Update");
                            $("#btnReset").hide();
                            $("#tblLogin").hide();
                            $("#tblLogout").show();
                            $("#btnsubmitOrder").show();
                            document.getElementById("lblName").innerHTML = $("#txtFirstName").val() + " " + $("#txtLastName").val();
                        }
                        else {
                            alert("Record updated successfully.");
                        }
                    },
                    "json"
                );
        }
        return false;
    });
});

function ClearForm() {
    $("#tblLogin").show();
    $("#tblLogout").hide();
    $("form")[0].reset();
}

function RegisterNewCustomer() {
    ClearForm();
    $('#customerInformation').show();
    $("#btnRegister").val("Register");
    $("#btnReset").show();
    $("#btnsubmitOrder").hide();
}

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
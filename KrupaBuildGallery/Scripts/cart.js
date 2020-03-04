function AddtoCartfunc(itemid,qty) {
    var URL = '/Client/Cart/AddtoCart';
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL + "?ItemId=" + itemid + "&Qty=" + qty,
        success: function (result) {
            if (result == "notfound") {
               alert("Product Not Found");
            }         
            $.ajax({
                url: '/Client/Cart/CartItemsListTop',
                type: "post",
                dataType: "html",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    //success
                    $("#ulCartitems").html(data); //populate the tab content.

                },
                error: function (e) {
                    alert("error");
                }
            });
        },
        error: function (resultData) {
            console.log("error");
            
        }
    });
}

function PlaceOrder(razorpay_payment_id, razorpay_order_id, razorpay_signature) {
    var URL = '/Client/Checkout/PlaceOrder';
    var formId = $("#frmcheckout");
    var data = $("#frmcheckout").serializeArray();
    data.push({ name: 'razorpay_payment_id', value: razorpay_payment_id });
    data.push({ name: 'razorpay_order_id', value: razorpay_order_id });
    data.push({ name: 'razorpay_signature', value: razorpay_signature });
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL,
        dataType: 'json',
        data: data,
        success: function (result) {
            if (result.indexOf("Success") >= 0) {
                var arryy = result.split('^');
                location.href = location.origin + "/client/checkout/ordersuccess?Id=" + arryy[1];
            }
            else {
                alert(result);
                location.href = location.origin + "/checkout";
                StopLoading();
            }
            
        }, 
        error: function (resultData) {
            console.log("error");
            StopLoading();
        }
    });
}
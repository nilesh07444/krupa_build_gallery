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
        },
        error: function (resultData) {
            console.log("error");
            
        }
    });
}

function PlaceOrder(razorpay_payment_id, razorpay_order_id, razorpay_signature) {
    var URL = '/Checkout/PlaceOrder';
    var formId = $("#frmcheckout");
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL,
        data: formId.serialize() + "&razorpay_payment_id=" + razorpay_payment_id + "&razorpay_order_id=" + razorpay_order_id + "&razorpay_signature=" + razorpay_signature,
        success: function (result) {
          
        },
        error: function (resultData) {
            console.log("error");

        }
    });
}
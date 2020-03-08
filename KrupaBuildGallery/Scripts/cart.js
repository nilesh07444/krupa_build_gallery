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

$('.product-box a .ti-heart1 , .product-box a .fa-heart1').on('click', function () {

    $.notify({
        icon: 'fa fa-check',
        title: 'Success!',
        message: 'Item Successfully added in wishlist'
    }, {
        element: 'body',
        position: null,
        type: "info",
        allow_dismiss: true,
        newest_on_top: false,
        showProgressbar: true,
        placement: {
            from: "top",
            align: "right"
        },
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: 5000,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        },
        icon_type: 'class',
        template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1}</span> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress" data-notify="progressbar">' +
            '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
            '</div>'
    });
});
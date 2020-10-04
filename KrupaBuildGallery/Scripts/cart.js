function AddtoCartfunc(variantid,itemid,qty,iscash) {
    if (iscash == "False") {
        addtoCart(variantid,itemid, qty,"false");        
    }
    else {
        $("#cashondeliverymodal").modal("show");
        $("#cashondeliverymodal #hdncscartitmid").val(itemid);
        $("#cashondeliverymodal #hdncscartqty").val(qty);
        $("#cashondeliverymodal #hdncscartitmidvarint").val(variantid);
        
    }
}

function AddtoCartfuncCombo(comboid,qty,iscash) {
    if (iscash == "False") {
        addtoCartCombo(comboid, qty, "false");
    }
    else {
        $("#cashondeliverymodalcombo").modal("show");
        $("#cashondeliverymodalcombo #hdnCartComboId").val(comboid);
        $("#cashondeliverymodalcombo #hdnComboQty").val(qty);
    }
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

function msgdisplay(msg) {
    $.notify({
        icon: 'fa fa-check',
        title: 'Success!',
        message: msg
    }, {
        element: 'body',
        position: null,
        type: "success",
        allow_dismiss: true,
        newest_on_top: false,
            showProgressbar: false,
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
}

function msgdisplayFail(msg) {
    $.notify({
        icon: 'fa fa-check',
        title: 'Fail!',
        message: msg
    }, {
        element: 'body',
        position: null,
        type: "warning",
        allow_dismiss: true,
        newest_on_top: false,
            showProgressbar: false,
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
}

//$('.product-4').slick({
//    infinite: true,
//    speed: 300,
//    slidesToShow: 4,
//    slidesToScroll: 4,
//    autoplay: true,
//    autoplaySpeed: 3000,
//    responsive: [
//        {
//            breakpoint: 1200,
//            settings: {
//                slidesToShow: 3,
//                slidesToScroll: 3
//            }
//        },
//        {
//            breakpoint: 991,
//            settings: {
//                slidesToShow: 2,
//                slidesToScroll: 2
//            }
//        },
//        {
//            breakpoint: 420,
//            settings: {
//                slidesToShow: 1,
//                slidesToScroll: 1
//            }
//        }
//    ]
//});

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
            showProgressbar: false,
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
var opts;
var bypy;
function CheckItemExists(bypymnt, optionss, e) {
    opts = optionss;
    bypy = bypymnt;
    var iscsh = "false";
    if($("#isCashondelivery").length > 0 && $("#isCashondelivery").val() == "True") {
        iscsh = "true";
    }
    var promcd = $("#PromoCode").val();
    var addretitle = $("#AddressTitle").val();
    var URL = '/Client/Checkout/CheckItemsinStock?IsCash=' + iscsh + '&PromoCode=' + promcd + '&AddressTitle=' + addretitle;
    jQuery.ajax({
        type: 'POST',
        async: false,
        url: URL,
        success: function (result) {
            console.log("checkexist");
            if (result == "OutofStock") {
                msgdisplayFail("Item is out of stock");
                setTimeout(function () {
                    location.href = location.origin + "/cart";
                }, 500);                 
            }
            else if (result == "Address Title Already Exist")
            {
                msgdisplayFail("Address Title Already Exist");
                return false;
            }
            else if (result == "Invalid Promo Code" || result == "Promo Code Usage Over" || result == "Promo Code Expired") {
                msgdisplayFail(result);
                return false;
            }
            else {       
                if ($("#hasFreeItemsinOrder").val() == "true") {
                    $("#freeitems").modal("show");
                }
                else {
                    if (bypymnt == "ByOther") {
                        var rzp1 = new Razorpay(optionss);
                        rzp1.open();
                        e.preventDefault();
                    }
                    else if (bypymnt == "ByCredit") {
                        StartLoading();
                        PlaceOrder("ByCredit", "", "");
                    }
                }
               
            }
        },
        error: function (resultData) {
            console.log("error");
            return false;
        }
    });
}

function addtosecondcart() {
    var itemid = $("#outofstockmodl #hdnsecndcartitmid").val();
    var qty = $("#outofstockmodl #hdnsecndcartqty").val();    
    var varintid = $("#outofstockmodl #hdnsecndcartqtyvarint").val();    
    var URL = '/Client/Cart/AddtoSecondCart';
    $("#outofstockmodl").modal("hide");
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL + "?VarintId=" + varintid + "&ItemId=" + itemid + "&Qty=" + qty,
        success: function (result) {            
                msgdisplay("Item Successfully added to your second cart");            
         
            // if (result == "notfound") {
            //  alert("Product Not Found");
            // }         
            $.ajax({
                url: '/Client/Cart/SecondCartItemsListTop',
                type: "post",
                dataType: "html",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    //success
                    $("#ulCartitemssecond").html(data); //populate the tab content.

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

function addtoCart(variantid,itemid, qty, iscash) {
    var URL = '/Client/Cart/AddtoCart';
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL + "?VarintId=" +variantid+"&ItemId=" + itemid + "&Qty=" + qty + "&IsCash=" + iscash,
        success: function (result) {
            if (result == "OutofStock") {
               // msgdisplayFail("Item is out of stock can not add to cart");
                $("#outofstockmodl").modal("show");
                $("#outofstockmodl #hdnsecndcartitmid").val(itemid);
                $("#outofstockmodl #hdnsecndcartqty").val(qty);
                $("#outofstockmodl #hdnsecndcartqtyvarint").val(variantid);
                
                return false;
            }
            else {
                msgdisplay("Item Successfully added to your cart");
            }

            // if (result == "notfound") {
            //  alert("Product Not Found");
            // }         
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

function addtoCartCashOndelivery(iscash) {
    var itemid = $("#cashondeliverymodal #hdncscartitmid").val();
    var qty = $("#cashondeliverymodal #hdncscartqty").val();    
    var varintid = $("#cashondeliverymodal #hdncscartitmidvarint").val();    
    if (iscash == "yes") {
        $("#cashondeliverymodal").modal("hide");
        addtoCart(varintid,itemid, qty,"true");      
    }
    else {
        $("#cashondeliverymodal").modal("hide");
        addtoCart(varintid,itemid, qty,"false");      
    }
}

function addtoCartCombo(comboid,qty, iscash) {
    var URL = '/Client/Cart/AddtoCartCombo';
    jQuery.ajax({
        type: 'POST',
        async: true,
        url: URL + "?ComboId=" + comboid + "&Qty=" + qty + "&IsCash=" + iscash,
        success: function (result) {
            if (result == "OutofStock") {
                msgdisplayFail("Combo Item is out of stock can not add to cart");                
                return false;
            }
            else {
                msgdisplay("Combo Items Successfully added to your cart");
            }

            // if (result == "notfound") {
            //  alert("Product Not Found");
            // }         
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

function addtoCartCashOndeliveryCombo(iscash) {
    var comboid = $("#cashondeliverymodalcombo #hdnCartComboId").val();
    var qty = $("#cashondeliverymodalcombo #hdnComboQty").val();
   
    if (iscash == "yes") {
        $("#cashondeliverymodalcombo").modal("hide");
        addtoCartCombo(comboid, qty, "true");
    }
    else {
        $("#cashondeliverymodalcombo").modal("hide");
        addtoCartCombo(comboid, qty, "false");
    }
}

function GetFreeItems(istru) {
    if (istru == "true") {
        $("#IncludeFreeItems").val("true");
    }
    if (bypy == "ByOther") {
        var rzp1 = new Razorpay(opts);
        rzp1.open();
        e.preventDefault();
    }
    else if (bypy == "ByCredit") {
        StartLoading();
        PlaceOrder("ByCredit", "", "");
    }
}
    function randomString() {
        var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
        var string_length = Math.floor((Math.random() * 5) + 1);
        var randomstring = '';
        for (var i = 0; i < string_length; i++) {
            var rnum = Math.floor(Math.random() * chars.length);
            randomstring += chars.substring(rnum, rnum + 1);
        }
        $('#spnstring').html(randomstring);
    }

//get file size


//get file path from client system

function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}



$(function () {
    $("#ImagePhoto").change(function () {
        var file = getNameFromPath($(this).val());
        if (file != null) {
            var extension = file.substr((file.lastIndexOf('.') + 1));
            switch (extension) {
                case 'jpg':
                    flag = true;
                case 'jpeg':
                    flag = true;
                case 'JPG':
                    flag = true;
                case 'png':
                    flag = true;
                case 'gif':
                    flag = true;
                    break;
                default:
                    flag = false;
            }
        }

        if (flag == false) {
            $("#validationTxt").text("You can upload only jpg,jpeg,png,gif extension file");
            return false;
        }
        else {
            var size = 0;
            if (size > 3) {
                $("#validationTxt").text("You can upload file up to 1 MB");
            }
            else {
                $("#validationTxt").text("");
            }
        }
    });
});

  

$(document).ready(function () {
    $("#Sub_Category_id").prop("disabled", true);

    $("#Category_id").change(function () {

        if ($("#Category_id").val() != "") {
            var menuid = $("#Category_id").val();
            $.post('@Url.Action("LoadSubCategory", "Admin")', { "menuid": menuid },
                   function (data) {
                       $("#Sub_Category_id").empty();
                       $.each(data, function (id, option) {
                           $("#Sub_Category_id").append($('<option></option>').val(option.Mnu_id).html(option.Mnu_Name));
                       });
                       $("#Sub_Category_id").prop("disabled", false);
                   });
        }
    })

    $('#Product_Name').keypress(function (event) {
        if(($("#Category_id").val() == 1) && ($("#brand_id").val() < 1))
        {
                  
            document.getElementById("divBrnd").style.visibility = "visible";
        }
        else {
            document.getElementById("divBrnd").style.visibility = "hidden";

        }
             

    })
    $('#Product_Price').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
        if (($(this).val().indexOf('.') != -1) && ($(this).val().substring($(this).val().indexOf('.'), $(this).val().indexOf('.').length).length > 2)) {
            event.preventDefault();
        }
    })
    $('#Product_Price').keyup(function (event) {
        var value = $('#Product_offer').val();
        if (($("#offer_id").val() == 1) && (value > 100)) {
            document.getElementById("divAT").style.visibility = "visible";
        }
        else {
            document.getElementById("divAT").style.visibility = "hidden";

        }
        var pricevalue = $('#Product_Price').val();
        if (($("#offer_id").val() == 2) && (value > pricevalue)) {
            document.getElementById("divrs").style.visibility = "visible";
        }
        else {
            document.getElementById("divrs").style.visibility = "hidden";

        }
    })
    $("#offer_id").change(function () {

        if ($("#offer_id").val() != "") {
            var value = $('#Product_offer').val();
            if (($("#offer_id").val() == 1) && (value > 100)) {
                document.getElementById("divAT").style.visibility = "visible";
            }
            else {
                document.getElementById("divAT").style.visibility = "hidden";

            }
            var pricevalue = $('#Product_Price').val();
            if (($("#offer_id").val() == 2) && (value > pricevalue)) {
                document.getElementById("divrs").style.visibility = "visible";
            }
            else {
                document.getElementById("divrs").style.visibility = "hidden";

            }

        }
    })
    $('#Product_offer').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
        if ($(this).val().length > 5) {
            event.preventDefault();
        }

    })

    $('#Product_offer').keyup(function (event) {
        var value = $('#Product_offer').val();
        if (($("#offer_id").val() == 1) && (value > 100)) {
            document.getElementById("divAT").style.visibility = "visible";                 
        }
        else {
            document.getElementById("divAT").style.visibility = "hidden";
                 
        }
             
        var pricevalue = $('#Product_Price').val();
          
        if (($("#offer_id").val() == 2) && (value > pricevalue)) {
            document.getElementById("divrs").style.visibility = "visible";               
        }
        else {
            document.getElementById("divrs").style.visibility = "hidden";
                 
        }
    })
    $('#Product_offer').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
        if ($(this).val().length > 5)  {
            event.preventDefault();
        }
            
            
    })

    $("#brand_id").change(function () {

        if ($("#brand_id").val() != "") {
            document.getElementById("divBrnd").style.visibility = "hidden";
            var text = "";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < 3; i++) {
                text += possible.charAt(Math.floor(Math.random() * possible.length));
            }

                    
                   
            // var menuid = $("#offer_id").val();
            $("#Product_Code").val($("#brand_id option:selected").text().substring(0, 3).toUpperCase() + "-" + text.toUpperCase());



        }
    })

    $("#Category_id").change(function () {

        if ($("#Category_id").val() != "") {

            var text = "";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < 3; i++) {
                text += possible.charAt(Math.floor(Math.random() * possible.length));
            }



            // var menuid = $("#offer_id").val();
            $("#Product_Code").val($("#Category_id option:selected").text().substring(0, 3).toUpperCase() + "-" + text.toUpperCase());



        }
    })
           
          

});


$("#myform").submit(function (e) {
    $("#loading").show();
});
$(window).unload(function () {
    $("#loading").hide();
});
$(document).ready(function () {
    $('#county').change(function () {
        var county = $(this).val();

        //alert(parseInt(lJobKey) + "=" + checkboxInput.checked)
        $('#issdt').hide();
        $('#loadingmessage1').show();
        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: "/Reports/issdate",  //'@Url.Action("issdate", "percentCalcs")',
            data: { county: county },
            success: function (data) {

                var items = '<option>Select Issue Date</option>';
                $.each(data, function (index, itemData) {
                    items += "<option value='" + itemData.Value + "'>" + itemData.Text + "</option>";

                });
                $('#loadingmessage1').hide();
                $('#issdt').show();
                $('#issdt').html(items);


            }
        });
        $('#loadingmessage').show();
        $('#lastdt').hide();
        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: "/Reports/lastdate",  // '@Url.Action("lastdate", "percentCalcs")',
            data: { county: county },
            success: function (mdata) {

                lstupdate = mdata;

                //});
                $('#lastdt').show();
                $('#lastdt').html(lstupdate);
                $('#loadingmessage').hide();

            }
        });
    });
});
$("#statusform").submit(function (e) {
    $("#loadingmessage1").show();
});

$(document).ready(function () {
    $('#Township').change(function () {
        $("#loadingup").show();
        $('#partialPlaceHolder').hide();
        /* Get the selected value of dropdownlist */
        var selectedID = $(this).val();
        // $.ajax({
        //    type: 'GET',
        //    dataType: 'json',
        //    url: "/Reports/lastdate",  // '@Url.Action("lastdate", "percentCalcs")',
        //    data: { county: county },
        //    success: function (mdata) {

        //        lstupdate = mdata;

        //        //});
        //        $('#lastdt').show();
        //        $('#lastdt').html(lstupdate);
        //        $('#loadingmessage').hide();

        //    }
        //});
        $('#download').show();
        $('#partialPlaceHolder').load("/NewJersey/_results/?selectedID=" + encodeURIComponent(selectedID))
        $('#partialPlaceHolder').hide();
        $('#partialPlaceHolder').fadeIn(1000);
        $("#loadingup").hide();
            })
        });

//    });
//});
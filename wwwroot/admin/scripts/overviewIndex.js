function initializeDataTable() {
    $('#table1').DataTable({
        order: [[14, "asc"]],
        dom: "Bfrtip",
        buttons: [],
        scrollX: true,
        columnDefs: [{
            orderable: false,
            className: 'select-checkbox',
            targets: 0
        }],
        select: {
            style: 'multi',
            selector: 'td:first-child',
        },
    });
}

function load_params() {
    $('#listbody').html("<span class='fa-stack fa-lg' style='position:static;height:0em;'>\n\
                                            <i class='fa fa-spinner fa-spin fa-stack-2x fa-fw'></i>\n\
                                       </span>");

    var dateRange = $('.drp').val().split("-");
    var startDate = dateRange[0].trim();
    var endDate = dateRange[1].trim();

    $.ajax({
        url: "Overtime/getOvertimes?startDate=" + startDate + '&endDate=' + endDate,
        method: "get",
        datatype: "html",
        success: function (result) {

            $('#list').html(result);

            if ($.fn.DataTable && $('#table1').length) {
                $('#table1').DataTable().destroy(); // Destroy previous instance
                initializeDataTable();
            } else {
                $('#list').html("<div class='alert alert-warning'>No data available for the selected date range.</div>");
            }
        },
        error: function () {
            $('#list').html("<div class='alert alert-danger'>An error occurred while loading data. Please try again later.</div>");
        }
    });
}

$(document).ready(function () {
    if ($.fn.DataTable) {
        initializeDataTable();
    } else {
        console.error("DataTables library is not loaded.");
    }

    $('#Approve').click(function () {
        if (!$.fn.DataTable || !$.fn.DataTable.isDataTable('#table1')) {
            if (confirm("No Data Selected")) {
                location.reload();
            }
            return;
        }

        var table = $('#table1').DataTable();
        var checked_rows = table.rows('.selected').data();
        var data = [];

        for (var i = 0; i < checked_rows.length; i++) {
            var overId = checked_rows[i][1].replace("OVER", "");
            data.push(overId);
        }

        if (data.length > 0) {
            $.ajax({
                url: "/Overtime/Approve",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (result) {
                    alert("Data has been Approved");
                    location.reload();
                },
                error: function () {
                    alert("An error occurred while approving data. Please try again later.");
                }
            });
        } else {
            if (confirm("No Data Selected")) {
                location.reload();
            }
        }
    });

    let currentMonthYear = moment(new Date()).format("MM/YYYY");
    let startDate = "01/" + currentMonthYear;
    let endDate = moment(new Date()).add(1, 'd').format("DD/MM/YYYY");
    let dateValue = startDate + " - " + endDate;

    //initial value;
    $('.drp').val(dateValue);
    // Daterange picker
    $('.drp').daterangepicker({
        format: 'DD/MM/YYYY',
        startDate: startDate,
        endDate: new Date()
    });

    $('#add').click(function () {
        window.open(getOvertimeFormUrl(), "_blank");
    });
});

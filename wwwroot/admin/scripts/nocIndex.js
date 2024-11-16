$(function () {
    $("#table1").DataTable({
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ["excel"],
        scrollX: true
    });


    $('input[name="date"]').datetimepicker({
        format: "DD/MM/YYYY hh:mm",
        showClose: true
    });


    $('input[name="dueDate"]').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });


    $('input[name="date"]').datetimepicker({
        //let date = new Date();
        //date.toLocaleTimeString('id-ID', { hour12: false });
        format: "DD/MM/YYYY hh:mm",
        showClose: true
    });


    $('input[name="dueDate"]').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
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
});



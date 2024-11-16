$(document).ready(function () {
    initializeSelect2();
    initializeDataTable();
});

function initializeSelect2() {
    $("#userName").select2({
        placeholder: "Pilih Pekerja",
        allowClear: true
    });

    $("#bulan").select2({
        placeholder: "Pilih Bulan",
        allowClear: true
    });
}

function initializeDataTable() {
    if ($('#table1').length) {
        $('#table1').DataTable({
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [],
            scrollX: true
        });
    } else {
        console.error("Table element #table1 not found.");
    }
}

const baseUrl = getBaseUrl();
const urlDownload = `${baseUrl}/api/Sdm/export-excel`;

function download_recap() {
    const currentYear = new Date().getFullYear();
    const bulan = $('#bulan').val();
    const username = $('#userName').val();
    let tahun = $('#tahun').val() || currentYear;

    if (!bulan || !username) {
        alert("Bulan dan Pekerja harus dipilih");
        return;
    }

    const url = `${urlDownload}?bulan=${bulan}&username=${username}&tahun=${tahun}`;
    window.location.assign(url);
}

function load_params() {
    const currentYear = new Date().getFullYear();
    const bulan = $('#bulan').val();
    const username = $('#userName').val();
    let tahun = $('#tahun').val() || currentYear;

    if (!bulan || !username) {
        alert("Bulan dan Pekerja harus dipilih");
        return;
    }

    $('#listbody').html(`
        <span class='fa-stack fa-lg' style='position:static;height:0em;'>
            <i class='fa fa-spinner fa-spin fa-stack-2x fa-fw'></i>
        </span>
    `);

    $.ajax({
        url: `getRecap?bulan=${bulan}&username=${username}&tahun=${tahun}`,
        method: "get",
        datatype: "html",
        success: function (result) {
            $('#list').html(result);
            if ($('#table1').length && $.fn.DataTable.isDataTable('#table1')) {
                $('#table1').DataTable().destroy();
            }
            initializeDataTable();
        },
        error: function (xhr, status, error) {
            console.error("AJAX request failed: ", status, error);
            $('#list').html("<div class='alert alert-danger'>Failed to load data. Please try again later.</div>");
        }
    });
}

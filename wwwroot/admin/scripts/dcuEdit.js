$(function () {
    baseUrl = getBaseUrl();
    let urlDCU = `${baseUrl}/api/Dcu/form`;
    let urlCommon = `${baseUrl}/api/common`;
    let dcuid = $(`#DCUID`).val();

    //$('#dcu-form').validate({
    //    rules: {
    //        files: {
    //            accept: "image/jpg,image/jpeg,image/png"
    //        },
    //        entryDate: {
    //            required: true
    //        },
    //        nama: {
    //            required: true
    //        },
    //        jenispekerjaan: {
    //            required: true
    //        },
    //        sistole: {
    //            required: true
    //        },
    //        diastole: {
    //            required: true
    //        },
    //        nadi: {
    //            required: true
    //        },
    //        suhu: {
    //            required: true
    //        }
    //    },
    //    messages: {
    //        files: "Only accept .jpg, .jpeg, or .png format",
    //        nama: "Nama wajib terisi",
    //        jenispekerjaan: "Pilih Jenis Pekerjaan",
    //        sistole: "Isi dengan Angka",
    //        diastole: "Isi dengan Angka",
    //        nadi: "Isi dengan Angka",
    //        suhu: "Isi dengan Angka"
    //    }
    //});

    if ($(`select[name="jenispekerjaan"]`).val() == 7) {
        $(`#hiddenAction`).attr("hidden", false);
        $(`#hiddenCondition`).attr("hidden", true);
    } else {
        $(`#hiddenAction`).attr("hidden", true);
        $(`#hiddenCondition`).attr("hidden", true);
    }

    $(`select[name="jenispekerjaan"]`).on("change", function (e) {
        if ($(this).val() == 7) {
            $(`#hiddenAction`).attr("hidden", false);
            $(`#hiddenCondition`).attr("hidden", true);
            $(`select[name="other"]`).val("0");
        } else {
            $(`#hiddenAction`).attr("hidden", true);
            $(`#hiddenCondition`).attr("hidden", true);
            $(`select[name="other"]`).val("0");
        }
    });
});

function extensionHandle(ext) {
    switch (ext.toString().toLowerCase()) {
        case "png":
            return "image/png";
        default:
            return "image/jpeg";
    }
}
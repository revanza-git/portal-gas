$(function () {
    baseUrl = getBaseUrl();
    let urlNoc = `${baseUrl}/api/Noc`;
    let urlCommon = `${baseUrl}/api/common`;
    let nocid = $(`#NOCID`).val();

    $.ajax({
        url: urlNoc + "/" + nocid,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.ajax({
                url: urlCommon + "/unsafeaction-list",
                contentType: "application/json",
                type: "GET",
                dataType: "json",
                success: function (res) {
                    for (var i = 0; i < res.length; i++) {
                        let name = res[i].deskripsi;
                        let value = res[i].unsafeActionID;

                        if (data["unsafeAction"] == value) {
                            $option = $("<option></option>")
                                .attr("value", value)
                                .text(name)
                                .prop("selected", true);
                        } else {
                            $option = $("<option></option>").attr("value", value).text(name);
                        }

                        $('select[name="unsafeAction"]').append($option).trigger("change");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("Status: " + textStatus);
                    alert("Error: " + errorThrown);
                },
            });

            $.ajax({
                url: urlCommon + "/unsafecondition-list",
                contentType: "application/json",
                type: "GET",
                dataType: "json",
                success: function (res) {
                    for (var i = 0; i < res.length; i++) {
                        let name = res[i].deskripsi;
                        let value = res[i].unsafeConditionID;

                        if (data["unsafeCondition"] == value) {
                            $option = $("<option></option>")
                                .attr("value", value)
                                .text(name)
                                .prop("selected", true);
                        } else {
                            $option = $("<option></option>").attr("value", value).text(name);
                        }

                        $('select[name="unsafeCondition"]')
                            .append($option)
                            .trigger("change");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("Status: " + textStatus);
                    alert("Error: " + errorThrown);
                },
            });

            $.ajax({
                url: urlCommon + "/clsr-list",
                contentType: "application/json",
                type: "GET",
                dataType: "json",
                success: function (res) {
                    for (var i = 0; i < res.length; i++) {
                        let name = res[i].deskripsi;
                        let value = res[i].clsrID;

                        if (data["clsr"] == value) {
                            $option = $("<option></option>")
                                .attr("value", value)
                                .text(name)
                                .prop("selected", true);
                        } else {
                            $option = $("<option></option>").attr("value", value).text(name);
                        }

                        $('select[name="clsr"]').append($option).trigger("change");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("Status: " + textStatus);
                    alert("Error: " + errorThrown);
                },
            });

      $(":submit").on("click", function (e) {
        if (nocid != null && nocid != "") {
          let entryDate = $(`#EntryDate`).val();
          let dueDate = $(`#DueDate`).val();
          let file = $(`input[name="files"]`).val();
          let fileName;
          let fileExt;

                    if (file != "") {
                        let filePath = $(`input[name="files"]`).val().split("\\")[2];
                        fileExt = filePath.split(".").splice(-1);
                        fileName = nocid + "." + fileExt;
                    } else {
                        fileExt = data["photo"].split(".").splice(-1);
                        fileName = data["photo"];
                    }

                    data = {
                        nocid: $(`#NOCID`).val(),
                        photo: fileName,
                        contentType: extensionHandle(fileExt),
                        date: moment().format("YYYY-MM-DD"),
                        time: moment().format("HH:mm"),
                        lokasi: $(`#Lokasi`).val(),
                        daftarPengamatan: $(`#DaftarPengamatan`).val(),
                        unsafeAction: $(`select[name="unsafeAction"]`).val(),
                        unsafeCondition: $(`select[name="unsafeCondition"]`).val(),
                        clsr: $(`select[name="clsr"]`).val(),
                        deskripsi: $(`#Deskripsi`).val(),
                        tindakan: $(`#Tindakan`).val(),
                        rekomendasi: $(`#Rekomendasi`).val(),
                        prioritas: $(`#Prioritas`).val(),
                        status: $(`#Status`).val(),
                        entryDate: moment(entryDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
                        dueDate: moment(dueDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
                        namaObserver: "",
                        divisiObserver: "",
                    };

                    $.ajax({
                        url: urlNoc + "/set/" + nocid,
                        contentType: "application/json",
                        type: "PUT",
                        data: JSON.stringify(data),
                        success: function (result) {
                            console.log("update success");
                        },
                    });
                } else {
                    $.ajax({
                        url: urlNoc + "/set/" + nocid,
                        contentType: "application/json",
                        type: "PUT",
                        data: JSON.stringify(data),
                        success: function (result) {
                            console.log("update success");
                        },
                    });
                    console.log("false");
                }
            });
        },
        error: function (xhr, textStatus, errorThrown) {
            alert("Status: " + textStatus);
            alert("Error: " + errorThrown);
        },
    });

  if ($(`select[name="DaftarPengamatan"]`).val() == 2) {
    $(`#hiddenAction`).attr("hidden", false);
    $(`#hiddenCondition`).attr("hidden", true);
  } else if ($(`select[name="DaftarPengamatan"]`).val() == 3) {
    $(`#hiddenCondition`).attr("hidden", false);
    $(`#hiddenAction`).attr("hidden", true);
  } else {
    $(`#hiddenAction`).attr("hidden", true);
    $(`#hiddenCondition`).attr("hidden", true);
  }

  $(`select[name="DaftarPengamatan"]`).on("change", function (e) {
    if ($(this).val() == 2) {
      $(`#hiddenAction`).attr("hidden", false);
      $(`#hiddenCondition`).attr("hidden", true);
      $(`select[name="unsafeCondition"]`).val("0");
    } else if ($(this).val() == 3) {
      $(`#hiddenCondition`).attr("hidden", false);
      $(`#hiddenAction`).attr("hidden", true);
      $(`select[name="unsafeAction"]`).val("0");
    } else {
      $(`#hiddenAction`).attr("hidden", true);
      $(`#hiddenCondition`).attr("hidden", true);
      $(`select[name="unsafeCondition"]`).val("0");
      $(`select[name="unsafeAction"]`).val("0");
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

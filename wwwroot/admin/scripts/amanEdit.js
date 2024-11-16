$(function () {
  baseUrl = getBaseUrl();
  let amanId = $("#AmanID").val();
  let UserLoginUserName = $("#UserLoginUserName").val();
  let UserLoginDepartment = $("#UserLoginDepartment").val();
  let urlAman = `${baseUrl}/api/Aman`;
  let urlUser = `${baseUrl}/api/User`;
  let auditors;
  let correction;
  let startDate;
  let endDate;
  let creationDateTime;
  let data;

  $("#Auditors-edit").on("change", function () {
    auditors = $(this).val();
  });

    $("#CorrectionType").on("change", function () {
        console.log($(this).val());
    });

  if (amanId != null && amanId != "") {
    //for AMAN Module update
    startDate = $(`input[name="StartDate"]`).val();
    endDate = $(`input[name="EndDate-edit"]`).val();
    creationDateTime = $(`input[name="CreationDateTime"]`).val();

    getEditUsers(urlUser, urlAman, amanId);

      $("#submit-edit").on("click", function (e) {
          data = {
              amanID: amanId,
              startDate: moment(startDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
              source: $(`select[name="Source"]`).val(),
              location: $(`select[name="Location"]`).val(),
              findings: $(`textarea[name="Findings"]`).val(),
              recommendation: $(`textarea[name="Recommendation"]`).val(),
              responsible: $(`select[name="Responsible"]`).val(),
              verifier: $(`select[name="Verifier"]`).val(),
              priority: $(`select[name="Priority"]`).val(),
              endDate: moment(endDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
              status: $(`input[name="Status"]`).val(),
              correctionType: $(`select[name="CorrectionType"]`).val(),
              progress: $(`input[name="Progress"]`).val(),
              contentType: $(`input[name="ContentType"]`).val(),
              fileName: $(`input[name="FileName"]`).val(),
              department: $(`input[name="Department"]`).val(),
              creator: $(`input[name="Creator"]`).val(),
              creationDateTime: moment(creationDateTime, "DD/MM/YYYY HH:mm:ss").format(
                  "YYYY-MM-DDTHH:mm:ss"
              ),
              overdueNotif: $(`input[name="OverdueNotif"]`).val(),
          };
        data["classification"] = $(`input[type="radio"]:checked`).val();

      if ($("#Auditors-edit").val() == null) {
        e.preventDefault();
        $("#Auditors-text").text("The Auditor field is required");
      } else {
        e.preventDefault();
        data["auditors"] = auditors.join(", ");
        $("#Auditors-text").text("");
          amanUpdate(urlAman, data, "Aman");
      }
    });
  } else {
    //AMAN create
    startDate = $(`input[name="StartDate"]`).val();
    endDate = $(`input[name="EndDate"]`).val();
    creationDateTime;

    $.ajax({
      url: urlUser,
      contentType: "application/json",
      type: "GET",
      dataType: "json",
      success: function (res) {
        for (var i = 0; i < res.length; i++) {
          let name = res[i].name;
          let $option = $("<option></option>").attr("value", name).text(name);

          $("#Auditors-edit").append($option).trigger("change");
        }
      },
    });

    $("#submit-edit").on("click", function (e) {
      data = {
        startDate: moment(startDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
        endDate: moment(endDate, "DD/MM/YYYY").format("YYYY-MM-DD"),
        source: $(`select[name="Source"]`).val(),
        location: $(`select[name="Location"]`).val(),
        findings: $(`textarea[name="Findings"]`).val(),
        recommendation: $(`textarea[name="Recommendation"]`).val(),
        responsible: $(`select[name="Responsible"]`).val(),
        verifier: $(`select[name="Verifier"]`).val(),
        priority: $(`select[name="Priority"]`).val(),
        correctionType: $(`select[name="CorrectionType"]`).val(),
        status: 2,
        progress: 0,
        department: UserLoginDepartment,
        creator: UserLoginUserName,
        creationDateTime: moment().format("YYYY-MM-DD"),
        overdueNotif: 0,
        classification: $(`input[type="radio"]:checked`).val(),
      };

      if (validateData(data)) {
        e.preventDefault();
        if ($("#Auditors-edit").val() == null) {
          $("#Auditors-text").text("The Auditor field is required");
        } else {
          data["auditors"] = auditors.join(", ");
          $("#Auditors-text").text("");
          $("#submit-edit").text("Loading...");
          amanCreate(urlAman, data, "Aman");
        }
      } else {
        console.log("Data masih empty");
      }
    });
  }
});

function validateData(data) {
  let emptyCounter = 0;

  Object.entries(data).forEach(([key, value]) => {
    if (value === "" || value === null) emptyCounter += 1;
  });

  if (emptyCounter > 0) {
    return false;
  } else {
    return true;
  }
}

//CREATE
function amanCreate(url, data, source) {
  $.ajax({
    url: url + "/add",
    contentType: "application/json",
    type: "POST",
    data: JSON.stringify(data),
    success: function (result) {
      location.assign(`/${source}/Index`, "_self");
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert("Status: " + textStatus);
      alert("Error: " + errorThrown);
    },
  });
}
//EDIT
function amanUpdate(url, data, source) {
  $.ajax({
    url: url + "/set/" + data["amanID"],
    contentType: "application/json",
    type: "PUT",
    data: JSON.stringify(data),
    success: function (result) {
        console.log("update success");
        alert("update success");
        location.assign(`/${source}/Index`, "_self");
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert("Status: " + textStatus);
      alert("Error: " + errorThrown);
    },
  });
}

function getEditUsers(urlUser, urlAman, amanId) {
  $.ajax({
    url: urlUser,
    contentType: "application/json",
    type: "GET",
    dataType: "json",
    success: function (res) {
      handleEditUsers(res, urlAman, amanId);
    },
  });
}

function handleEditUsers(data, urlAman, amanId) {
  $.ajax({
    url: urlAman + "/" + amanId,
    contentType: "application/json",
    type: "GET",
    dataType: "json",
    success: function (res) {
      if (res.auditors != null) {
        let arr = res.auditors.split(", ");
        for (var i = 0; i < data.length; i++) {
          let name = data[i].name;
          let match = false;
          let $option;

          for (var y = 0; y < arr.length; y++) {
            if (arr[y] === name) {
              match = true;
            }
          }

          if (match) {
            $option = $("<option></option>")
              .attr("value", name)
              .text(name)
              .prop("selected", true);
          } else {
            $option = $("<option></option>").attr("value", name).text(name);
          }

          $("#Auditors-edit").append($option).trigger("change");
        }
      } else {
        for (var i = 0; i < data.length; i++) {
          let name = data[i].name;
          let $option = $("<option></option>").attr("value", name).text(name);

          $("#Auditors-edit").append($option).trigger("change");
        }
      }
    },
  });
}

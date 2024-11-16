$(function () {
  baseUrl = getBaseUrl();
  let amanId = $("#AmanID").val();

  let urlNoc = `${baseUrl}/api/Noc`;
  let urlAman = `${baseUrl}/api/Aman`;
  let auditors;

  getViewAuditors(urlAman, amanId);

  $.ajax({
    url: urlAman + "/" + amanId,
    contentType: "application/json",
    type: "GET",
    dataType: "json",
    success: function (res) {
      if (res["nocid"] !== null) {
        $("#noc-referer").val(res["nocid"]);
      }
    },
  });

  $("#amanClose").on("click", function (e) {
    e.preventDefault();
    $.ajax({
      url: urlAman + "/" + amanId,
      contentType: "application/json",
      type: "GET",
      dataType: "json",
      success: function (res) {
        res["status"] = 3;
        amanStatusUpdate(urlAman, res);
        if (res["nocid"] !== null) {
          updateAmanNocStatus(urlNoc, res["nocid"]);
        }
      },
      error: function (XMLHttpRequest, textStatus, errorThrown) {
        alert("Status: " + textStatus);
        alert("Error: " + errorThrown);
      },
    });
  });
});

function updateAmanNocStatus(url, id) {
  $.ajax({
    url: url + "/" + id,
    contentType: "application/json",
    type: "GET",
    dataType: "json",
    success: function (res) {
      res["status"] = 2;
      updateAmanNoc(url, res);
    },
  });
}

function updateAmanNoc(url, data) {
  $.ajax({
    url: url + "/set/" + data["nocid"],
    contentType: "application/json",
    type: "PUT",
    data: JSON.stringify(data),
    success: function (result) {
      console.log("update status noc success");
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert("Status: " + textStatus);
      alert("Error: " + errorThrown);
    },
  });
}
//VIEW
function getViewAuditors(url, amanId) {
  $.ajax({
    url: url + "/" + amanId,
    contentType: "application/json",
    type: "GET",
    dataType: "json",
    success: function (result) {
      if (result.auditors != null) {
        let arr = result.auditors.split(", ");

        for (var i = 0; i < arr.length; i++) {
          let $option = $("<option></option>")
            .attr("value", arr[i])
            .text(arr[i])
            .prop("selected", true);

          $("#Auditors-view").append($option).trigger("change");
        }
      }
    },
  });
}

function amanStatusUpdate(url, data) {
  $.ajax({
    url: url + "/set/status/" + data["amanID"],
    contentType: "application/json",
    type: "PUT",
    data: JSON.stringify(data),
    success: function (result) {
      console.log("update status aman success");
      location.assign(`/Aman/Index`, "_self");
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert("Status: " + textStatus);
      alert("Error: " + errorThrown);
    },
  });
}

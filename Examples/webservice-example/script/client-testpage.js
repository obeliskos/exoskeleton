window.addEventListener("load", function load(event) {
    window.removeEventListener("load", load, false);

    setTimeout(initializePage, 100);
},false);

function initializePage() {
  var params = {
      name: 'test'
  };

  $.ajax({
    url: "http://localhost:8080/Contacts/GetContactList.svc",
    type: "GET",
    cache: false,
    data: params,
    dataType: 'json',
    success: function (result) {
      console.dir(result);

      for (var i=0;i<result.length;i++){
         $('<option/>').val(result[i]).html(result[i]).appendTo('#ContactSelect');
      }        
    },
    error: function (reqest, status, error) {
      alert("error: " + error);
    }
  });
}

function contactSelected() {
  var contactName = $("#ContactSelect option:selected").text();
  
  requestContact(contactName);
}

function requestContact(contactName) {
  var params = {
      name: contactName
  };

  $.ajax({
    url: "http://localhost:8080//GetContact.svc",
    type: "POST",
    data: params,
    cache: false,
    dataType: 'json',
    success: function (result) {
      // now that we have contact info (as json), display in textarea
      document.getElementById('ContactResponseText').value = JSON.stringify(result, null, 2);
    },
    error: function (request, status, error) {
      console.dir("error: " + error);
    }
  });
}

function requestAboutUs() {
  $.ajax({
    url: "http://localhost:8080/About/Us.svc",
    type: "GET",
    cache: false,
    dataType: "html",
    success: function (result) {
      $("#AboutUsDiv").html(result);
    },
    error: function(request, status, error) {
      alert(error);
    }
  });
}
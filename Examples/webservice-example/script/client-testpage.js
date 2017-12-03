window.addEventListener("load", function load(event) {
    window.removeEventListener("load", load, false);

    setTimeout(initializePage, 100);
},false);

function initializePage() {
  $.ajax({
    url: "http://localhost:8080//GetContactList.svc",
    type: "GET",
    cache: false,
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
    error: function (reqest, status, error) {
      console.dir("error: " + error);
    }
  });
}

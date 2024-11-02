function getYear() {
    var currentDate = new Date();
    var currentYear = currentDate.getFullYear();
    document.querySelector("#displayYear").innerHTML = currentYear;
}

getYear();

function myMap() {
    var mapOptions = {
        center: { lat: 40.40204201502193, lng: 49.95351858465297 },
        zoom: 17,
    };

    var map = new google.maps.Map(document.getElementById('googleMap'), mapOptions);
}
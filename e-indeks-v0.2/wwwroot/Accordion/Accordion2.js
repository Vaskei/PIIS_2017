//document.querySelectorAll('.accordion').forEach(function (el, index) {
//    // Add event listener to all elements
//    el.addEventListener('click', function () {
//        el.classList.toggle('active');
//        el.nextElementSibling.style.display = el.nextElementSibling.style.display === 'block'
//            ? 'none'
//            : 'block';
//    });

//    // First element open
//    0 === index && (el.nextElementSibling.style.display = 'block');
//});

var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
    acc[i].onclick = function () {
        this.classList.toggle("active");
        var panel = this.nextElementSibling;
        if (panel.style.display === "block") {
            panel.style.display = "none";
        } else {
            panel.style.display = "block";
        }
    }
}


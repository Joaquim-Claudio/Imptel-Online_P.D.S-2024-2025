var SideNav = {
    expand: () => {

        if(window.innerWidth >= 768 && window.innerWidth < 992) {
            document.getElementById("sideNav").style.width = "45%";
        } else {
            document.getElementById("sideNav").style.width = "100%";
            document.body.style.overflow = "hidden";
        }
    },

    collapse: () => {
        document.getElementById("sideNav").style.width = "0";
        document.body.style.overflow = "auto";
    }
    }

    window.addEventListener("DOMContentLoaded", function(event) {

    Array.from(document.getElementsByClassName("sn-1")).forEach(link => {
        link.addEventListener("click", ()=> {
            SideNav.collapse();
        })
    })
});

$(document).ready(function() {
        
    $('.btn-ft-dropdown').on('click', function() {

        var parent = $(this).parent().parent();

        parent.siblings('.active').children('.dropdown-center')
        .children('.btn-ft-dropdown').children('.dropdown-icon.active').removeClass('active');

        parent.siblings('.active').removeClass('active');

        $('.collapse.show').removeClass('show');

        parent.toggleClass('active');
        $(this).children('.dropdown-icon').toggleClass('active');
    });

    $(".carousel-item:first").addClass("active");
});

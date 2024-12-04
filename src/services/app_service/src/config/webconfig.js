$(document).ready(function() {
    setMainContentHeight();

    $(window).resize(function() {
        setMainContentHeight();
    })

    $()

});


function setMainContentHeight() {
    $("main").css({
        height: `${$(document).height() + 300}px` 
    })
}
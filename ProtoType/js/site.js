$('[data-togglesection]').click(function (e) {
    $('[data-togglesection]').removeClass('active');
    $('[data-contentsection]').addClass('hidden-section');
    var section = $(this).data("togglesection");
    $(section).toggleClass('hidden-section');

    $('#intro').addClass('hidden-section');
    if ($(section).attr('data-showintro') != null)
        $('#intro').toggleClass('hidden-section');

    $(this).addClass('active');
});
var active = true;
$('[data-collapse]').click(function (e) {
    var collapseLink = $('[data-collapse]');
    
    if (!collapseLink.hasClass("collapsed")) {
        active = false;
        $('.panel-collapse').collapse('show');
        $('.faq-question').attr('data-toggle', '');
        
     
        $('[data-expandcollapsespan]').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        $('.expandcollapsefont').text("Collapse All Answers");
    }
    else {
        active = true;
        $('.panel-collapse').collapse('hide');
        $('.faq-question').attr('data-toggle', 'collapse');
                
        $('[data-expandcollapsespan]').removeClass("glyphicon-minus").addClass("glyphicon-plus");
        $('.expandcollapsefont').text("Expand All Answers");
    }

    collapseLink.toggleClass("collapsed");
});

$('.collapse').on('show.bs.collapse', function (e) {
    $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
}).on('hidden.bs.collapse', function () {
    $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
});

$('#accordion').on('show.bs.collapse', function () {
    if (active) $('#accordion .in').collapse('hide');
});
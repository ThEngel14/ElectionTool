$(document).ready(function() {
    $("#txtToken").keydown(function (event) {
        if (event.ctrlKey || event.altKey) {
            return true;
        }

        var isNumber = 96 <= event.keyCode && event.keyCode <= 105;
        var isChar = 65 <= event.keyCode && event.keyCode <= 90;

        if (isNumber || isChar) {
            var txt = $('#txtToken');
            var oldLength = txt.val().length;

            // cancel if length > 19 [4-4-4-4]
            if (oldLength >= 19) {
                return false;
            }

            // add key
            txt.val(txt.val() + event.key);

            var value = txt.val();
            var len = value.length;

            // add - if cursor is at the right position
            switch (len) {
            case 4:
            case 9:
            case 14:
                txt.val(value + "-");
            }

            // cancel insert because key is added before
            return false;
        }

        return true;
    });
});

$(document).ready(function () {
    $('.datatable-simple').dataTable({
        "bPaginate": false,
        "aaSorting": [],
        "bFilter": false,
        "bInfo": false,
        "bSortClasses": false,
        "language": {
            "zeroRecords": "Keine Daten verfügbar."
        }
    });
});

$(document).ready(function () {
    $('#winnerTable').dataTable({
        "lengthMenu": [ [10, 25, 50, -1], [10, 25, 50, "Alle"] ],
        "aaSorting": [],
        "aoColumns": [null,null,null,{ "bSortable": false },null,{ "bSortable": false}],
        "bSortClasses": false,
        "language": {
            "lengthMenu": "Zeige _MENU_ Einträge",
            "sSearch": "Suchen: ",
            "oPaginate": {
                "sNext": "Nächste",
                "sPrevious": "Vorherige"
            },
            "info": "Zeigt _START_ bis _END_ von _TOTAL_ Einträgen",
            "infoFiltered": "(gefiltert von _MAX_ Einträgen)",
            "zeroRecords": "Keine Einträge zum Anzeigen verfügbar"
        }
    });
});

$(document).ready(function () {
    $('#ueberhangTable').dataTable({
        "bPaginate": false,
        "aaSorting": [],
        "bSortClasses": false,
        "language": {
            "lengthMenu": "Zeige _MENU_ Einträge",
            "sSearch": "Suchen: ",
            "info": "Zeigt _START_ bis _END_ von _TOTAL_ Einträgen",
            "infoFiltered": "(gefiltert von _MAX_ Einträgen)",
            "zeroRecords": "Keine Einträge zum Anzeigen verfügbar"
        }
    });
});

$(document).ready(function () {
    $('#memberBundestagTable').dataTable({
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Alle"]],
        "aaSorting": [],
        "aoColumns": [null, null, { "bSortable": false }, null, null],
        "bSortClasses": false,
        "language": {
            "lengthMenu": "Zeige _MENU_ Einträge",
            "sSearch": "Suchen: ",
            "oPaginate": {
                "sNext": "Nächste",
                "sPrevious": "Vorherige"
            },
            "info": "Zeigt _START_ bis _END_ von _TOTAL_ Einträgen",
            "infoFiltered": "(gefiltert von _MAX_ Einträgen)",
            "zeroRecords": "Keine Einträge zum Anzeigen verfügbar"
        }
    });
});

$(document).ready(function () {
    $('#seatsTable').dataTable({
        "bPaginate": false,
        "aaSorting": [],
        "aoColumns": [{ "bSortable": false }, null, null, null, null],
        "bFilter": false,
        "bInfo": false,
        "bSortClasses": false,
        "language": {
            "zeroRecords": "Keine Daten verfügbar."
        }
    });
});
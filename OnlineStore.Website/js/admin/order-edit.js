var orderNoteTemplate = $('#EditOrderNoteTemplate').html(),
    $orderNotesList = $('#OrderNotesList'),
    $addOrderNote = $('#AddOrderNote'),
    $jsonNotes = $('#JSONNotes'),
    notes = [];

$addOrderNote.on('click', function () {
    var tmp = $(orderNoteTemplate);

    var Note = {
        ID: -1 * _.random(1000, 9999),
        Note: '',
    };

    tmp.data('Note', Note);
    notes.push(Note);

    $orderNotesList.append(tmp);

});

$orderNotesList.on('keyup change', 'textarea', function () {
    var $this = $(this),
        row = $this.closest('.orderNoteitem'),
        Note = row.data('Note');

    if ($this.hasClass('note')) {
        Note.Note = $this.val();
    }

    refreshJSONNotes();
});

function refreshJSONNotes() {
    $jsonNotes.val(JSON.stringify(notes));
}

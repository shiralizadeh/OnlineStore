$('.edit-comment').magnificPopup({
    type: 'iframe',
    midClick: true,
    removalDelay: 300,
    mainClass: 'mfp-fade'
});

config({
    DeleteUrl: '/My-Account/My-Comments/Delete'
});
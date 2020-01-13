var $score = $('.admin-score .score-parameter');

$score.find('select').barrating({
    theme: 'bars-movie',
    readonly: true
});

$score.find('select').each(function () {
    var $param = $(this);
    $param.barrating('set', $param.data('rate'));
});

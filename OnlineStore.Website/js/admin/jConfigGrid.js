var pagesizeKey = location.pathname + "_pagesize";

var jConfigGrid = {
    options: {
        ajaxUrl: '',
        customAjaxUrl: '',
        customAjaxSuccess: undefined,
        customAjaxError: undefined,
    },
    pageIndex: 0,
    pageSize: (localStorage && localStorage.getItem(pagesizeKey) ? localStorage.getItem(pagesizeKey) : 25),
    pageOrder: 'ID',
    orderAsc: true,
    init: function (options) {
        this.options = $.extend({}, this.options, options);

        $('.jPageSize').val(this.pageSize);

        this.getData(this.pageIndex, this.pageSize, this.pageOrder, this.orderAsc);
        this.searchEvent();
    },
    getData: function (pageIndex, pageSize, pageOrder, orderAsc) {
        var jConfigGrid = this;
        jConfigGrid.showLoading();

        var data = {
            pageIndex: pageIndex,
            pageSize: pageSize,
            pageOrder: pageOrder + ' ' + (orderAsc ? '' : ' DESC')
        };

        if (jConfigGrid.options.filter)
            data = $.extend({}, data, jConfigGrid.options.filter());

        $.ajax({
            url: this.options.ajaxUrl + 'Get',
            method: 'post',
            dataType: 'json',
            data: data,
            success: function (result) {
                jConfigGrid.getTemplate(result);
                jConfigGrid.pageIndex = result.PageIndex;
                jConfigGrid.makePaging(result.TotalPages, result.PageIndex + 1);
            },
            error: function () {

            },
            complete: function () {
                jConfigGrid.hideLoading();
            }
        });
    },
    showLoading: function () {
        $('.loading').fadeIn();
    },
    hideLoading: function () {
        $('.loading').fadeOut();
    },
    refresh: function (index) {
        if (index == 0)
            jConfigGrid.pageIndex = index;

        jConfigGrid.getData(jConfigGrid.pageIndex, jConfigGrid.pageSize, jConfigGrid.pageOrder, jConfigGrid.orderAsc);
    },
    getTemplate: function (result) {
        var jConfigGrid = this;
        var template = $('#jConfigGridTemplate').html();
        var gridTable = '';

        var Rows = result.Rows;

        $(Rows).each(function (index) {
            gridTable += jConfigGrid.templatePattern(template.replace('{{index}}', (index + 1) + result.PageIndex * result.PageSize), this);
        });

        $('.jconfigGrid tbody').empty().append(gridTable);

        this.setDeleteEvents();
        this.setCustomAjaxEvents();
    },
    templatePattern: function (template, item) {
        var row = template;
        for (var key in item) {
            var regx = new RegExp("{{(?:.*\\()?" + key + "(?:\\).*)?}}", "gm");
            var results = regx.exec(row);

            while (results) {
                for (var i = 0; i < results.length; i++) {
                    var match = results[i];

                    var found = match.replace('{{', '').replace('}}', '');
                    found = found.replace('(' + key + ')', '("' + item[key] + '")');

                    var data = '';

                    if (found == key)
                        data = item[key];
                    else
                        data = eval(found);

                    row = row.replace(match, data)
                }

                results = regx.exec(row);
            }
        }

        return row;
    },
    makePaging: function (totalPages, pageIndex) {
        var paging = [],
            i, j, k;

        var totalPages = parseInt(totalPages);

        i = pageIndex;
        j = pageIndex - 1;
        k = pageIndex + 1;

        while (j != 0 && j != i - 6) {
            paging.push(j);
            j--;
        }
        paging.reverse();

        paging.push(i);

        for (; k < totalPages + 1 && k < i + 6; k++) {
            paging.push(k);
        }

        var paginate = $('.jGridPaging ul');

        paginate.empty();
        if (pageIndex != 1)
            paginate.append('<li class="prev"><a href="#" data-index="' + (pageIndex - 1) + '">→</a></li>');
        for (var i in paging) {
            var item = paging[i];
            paginate.append('<li class="' + (item == pageIndex ? 'active' : '') + '"><a href="#" data-index="' + item + '">' + item + '</a></li>');
        }
        if (pageIndex != totalPages)
            paginate.append('<li class="next"><a href="#" data-index="' + (pageIndex + 1) + '">←</a></li>');

        this.setPagerEvents();
        this.setPageSizeEvents();
    },
    setPagerEvents: function () {
        var jConfigGrid = this;
        $('.jGridPaging ul').find('a').on('click', function (e) {
            e.preventDefault();
            var pageIndex = parseInt($(this).data('index')) - 1;
            jConfigGrid.getData(pageIndex, jConfigGrid.pageSize, jConfigGrid.pageOrder, jConfigGrid.orderAsc);
        });
    },
    setPageSizeEvents: function (e) {
        var jConfigGrid = this;
        $('.jPageSize').one('change', function () {
            jConfigGrid.pageSize = $(this).val();
            localStorage.setItem(pagesizeKey, jConfigGrid.pageSize)

            jConfigGrid.getData(0, jConfigGrid.pageSize, jConfigGrid.pageOrder, jConfigGrid.orderAsc);
        });
    },
    setDeleteEvents: function () {
        var jConfigGrid = this;
        $('.btn-jdelete').on('click', function (e) {
            e.preventDefault();

            if (!confirm('آیا مایل به حذف سطر مورد نظر هستید؟'))
                return;

            var $this = $(this),
                id = $this.closest('tr').data('id');

            $this.fadeOut().prev().fadeOut();

            $.ajax({
                url: jConfigGrid.options.ajaxUrl + 'Delete',
                method: 'POST',
                data: { ID: id },
                success: function (result) {
                    if (result.Success)
                        $this.closest('tr').css({ 'text-decoration': 'line-through' }).animate({ opacity: '0.6' });
                },
                error: function () {
                    $this.fadeIn().prev().fadeIn();
                },
                complete: function () {
                }
            });
        });
    },
    setCustomAjaxEvents: function () {
        var jConfigGrid = this;

        $('.btn-confirmed').on('click', function () {
            jConfigGrid.showLoading();

            var ids = [];
            $('.select-item:checked').each(function () {
                var $this = $(this),
                    id = $this.closest('tr').data('id');

                ids.push(id);
            });

            $.ajax({
                type: 'POST',
                url: jConfigGrid.options.customAjaxUrl,
                data: {
                    IDs: ids
                },
                success: function (response) {
                    if (response.Success) {
                        if (jConfigGrid.options.customAjaxSuccess)
                            jConfigGrid.options.customAjaxSuccess(response);
                    }
                    else {
                        if (jConfigGrid.options.customAjaxError)
                            jConfigGrid.options.customAjaxError(response);
                    }
                },
                error: function (response) {
                    if (jConfigGrid.options.customAjaxError)
                        jConfigGrid.options.customAjaxError(response);
                },
                complete: function () {
                    jConfigGrid.hideLoading();
                }
            });
        });
    },
    searchEvent: function () {
        var btnSearch = $('#btnSearch');

        btnSearch.on('click', function () {
            jConfigGrid.refresh(0);
        });
    }
};
















var jConfigDropDown = {
    init: function () {

    },
    Options: {
        PlaceHolder: 'انتخاب کنید...'
    }
};







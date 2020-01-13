/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    config.language = 'fa';
    config.htmlEncodeOutput = true;
    config.enterMode = CKEDITOR.ENTER_BR;

    config.filebrowserBrowseUrl = '/assets/Ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/assets/Ckfinder/ckfinder.html';
    config.filebrowserUploadUrl = '/assets/Ckfinder/ckfinder.html';
    config.filebrowserImageUploadUrl = '/assets/Ckfinder/ckfinder.html';

    config.allowedContent = true;

    config.height = 500;
};

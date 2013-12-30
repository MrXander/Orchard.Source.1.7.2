(function ($) {
    AdminUploadInitializer = function (options, strings) {
        options = jQuery.extend({
            maxFileSize: 10000000,
            errorShowTimeout: 10000,
            form: $('#file_upload'),
            table: $('#files'),
            fileTypeRegexp: /\.(png)|(jpg)|(jpeg)|(gif)$/i
        }, options);

        strings = jQuery.extend({
            cancel: 'Cancel',
            uploading: 'Uploading',
            name: 'Name',
            error: 'Upload image failed.',
            errorNotImage: 'Uploaded file is not image.',
            errorEmpty: 'Uploaded file is empty.',
            errorLarge: 'Uploaded file is too large. Max file size:',
            mb: 'Mb'
        }, strings);
        var currentIndex = 0;
        function showError(errorMessage) {
            var tr = $('<tr/>');
            var td = $('<td/>').text(errorMessage).addClass('upload-error').attr('colspan', 3)
                .delay(options.errorShowTimeout).fadeOut('slow', function () {
                    $(this).remove();
                });
            tr.append(td);
            options.table.append(tr);
        }

        options.form.fileUploadUI({
            uploadTable: options.table,
            downloadTable: options.table,
            cancelSelector: '.upload-cancel',
            buildUploadRow: function (files, index) {
                var tr = $('<tr/>');
                var cancelBtn = $('<a>').addClass('button upload-cancel').text(strings.cancel);
                $('<td/>').addClass('upload-progress-cell').appendTo(tr);
                $('<td/>').addClass('upload-info').text(strings.uploading + ' ' + files[index].name).appendTo(tr);
                $('<td/>').addClass().appendTo(tr)
                        .append(cancelBtn);
                return tr;
            },
            buildDownloadRow: function (result) {
                var tr = $('<tr/>');
                if (result.successfully) {
                    var img = $('<img/>').attr('src', result.thumb);
                    var labelName = $('<label/>').text(strings.name);
                    var inputName = $('<input/>').attr('name', 'photos[' + currentIndex + '].Name').val(result.name);
                    var hidden = $('<input/>').attr('name', 'photos[' + currentIndex + '].Id').attr('type', 'hidden').val(result.id);
                    tr.append($('<td/>').width(result.width).addClass('upload-image').append(img));
                    tr.append($('<td/>').addClass('upload-name').append(hidden).append(labelName).append(inputName));
                    tr.append($('<td/>'));

                    $('.save-button').show();
                    currentIndex++;
                }
                else {
                    showError(result.errorMessage);
                }
                return tr;
            },
            onError: function () {
                showError(strings.error);
            },
            initUpload: function (event, files, index, xhr, handler, callBack) {
                // Using the filename extension for our test,
                // as legacy browsers don't report the mime type
                if (!options.fileTypeRegexp.test(files[index].name)) {
                    showError(strings.errorNotImage);
                    return;
                }

                if (files[index].size === 0) {
                    showError(strings.errorEmpty);
                    return;
                }

                if (files[index].size > options.maxFileSize) {
                    showError(strings.errorLarge + ' ' + options.maxFileSize + ' ' + strings.mb);
                    return;
                }

                handler.initUploadRow(event, files, index, xhr, handler, function () {
                    handler.beforeSend(event, files, index, xhr, handler, function () {
                        handler.initUploadProgress(xhr, handler);
                        callBack();
                    });
                });
            },
            beforeSend: function (event, files, index, xhr, handler, callBack) {
                var start = false;
                if (!files.hasOwnProperty('uploadSequence')) {
                    // The files array is a shared object between the instances of an upload selection.
                    // We extend it with a custom array to coordinate the upload sequence:
                    files.uploadSequence = [];
                    files.uploadSequence.start = function (i) {
                        var next = this[i];
                        if (next) {
                            // Call the callback with any given additional arguments:
                            next.apply(null, Array.prototype.slice.call(arguments, 1));
                        }
                    };
                    start = true;
                }
                files.uploadSequence.push(callBack);
                if (start) {
                    files.uploadSequence.start(0);
                }
            },
            onComplete: function (event, files, index, xhr, handler) {
                if (files.hasOwnProperty('uploadSequence')) {
                    files.uploadSequence.splice(0, 1);
                    files.uploadSequence.start(0);
                }
            }
        });
    };
}
)(jQuery)
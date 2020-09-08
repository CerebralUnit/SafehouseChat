'use strict';
var UploadArea = function ()
{
    /**
     * @return {?}
     */
    function render()
    {
        var self = this;
       
        return self.state = {
            isDragging: false,
            requireConfirm: true
        },
            self.setState = function (newState) {
                for (var k in newState) {
                    self.state[k] = newState[k];
                }
            },
            self.dragOverTimeout = null,
            self.canDropFile = function () {

                return true;
            },
            self.isAllDropFiles = function (items) {
                /** @type {number} */
                var i = 0;
                for (; i < items.length; i++) {
                    try {
                        var javaFile = items[i].webkitGetAsEntry() || items[i].getAsEntry();
                        if (javaFile && !javaFile.isFile) {
                            return false;
                        }
                    }
                    catch (e) {
                        continue;
                    }
                }
                return true;
            },
            self.preventUnwantedDrop = function (event, suppressPageChange) {
                if (void 0 === suppressPageChange) {
                    /** @type {boolean} */
                    suppressPageChange = true;
                }
                var data = event.dataTransfer;
                if (null == data) {
                    return true;
                }
                /** @type {boolean} */
                var a = data.types instanceof Array && -1 !== data.types.indexOf("text/uri-list") && -1 === data.types.indexOf("application/json");
                /** @type {boolean} */
                var b = null != data.items && !self.isAllDropFiles(data.items);
                return !a && !b || (event.stopPropagation(), event.preventDefault(), data.effectAllowed = "none", data.dropEffect = "none", suppressPageChange && (self.setState(
                    {
                        isDragging: false
                    }), l.default.push(S.default,
                        {
                            title: I.default.Messages.UPLOAD_AREA_INVALID_FILE_TYPE_TITLE,
                            help: I.default.Messages.UPLOAD_AREA_INVALID_FILE_TYPE_HELP
                        })), false);
            },
            self.handleDragOver = function (event) {
                if (!self.preventUnwantedDrop(event)) {
                    return false;
                }
                var dataTransfer = event.dataTransfer;
                if (null != dataTransfer) {
                    $('body').addClass('upload-started');
                    event.stopPropagation();
                    event.preventDefault();
                    if (self.state.isDragging) {
                        if (self.state.isDragging && event.shiftKey !== !self.state.requireConfirm) {
                            self.setState(
                                {
                                    requireConfirm: !event.shiftKey
                                });
                        }
                    }
                    else {
                        if (dataTransfer.types instanceof DOMStringList && dataTransfer.types.contains("application/x-moz-file") || dataTransfer.types instanceof Array && -1 !== dataTransfer.types.indexOf("Files")) {
                            self.setState({
                                isDragging: true,
                                requireConfirm: !event.shiftKey
                            });
                        }
                    }
                    clearTimeout(self.dragOverTimeout);
                    /** @type {number} */
                    self.dragOverTimeout = setTimeout(function () {
                        self.setState(
                            {
                                isDragging: false,
                                requireConfirm: true
                            });
                    }, 1e3);
                }
            },
            self.handleDragLeave = function (event) {
                if (self.state.isDragging) {
                    event.stopPropagation();
                    event.preventDefault();
                    self.clearDragging();
                }
            },
            self.clearDragging = function () {
                self.setState(
                    {
                        isDragging: false,
                        requireConfirm: true
                    });
            $('body').removeClass('upload-started');
            },
            self.handleDrop = function (event) {
                if (!self.preventUnwantedDrop(event, true)) {
                    return false;
                }
                var data = event.dataTransfer;
                if (null == data) {
                    return true;
                }
                if (self.state.isDragging) {
                    event.preventDefault();
                    event.stopPropagation();
                    if (self.canDropFile() && null != channelId) {
                        self.promptToUpload(data.files,
                            channelId, true,
                            self.state.requireConfirm);
                    }
                    self.clearDragging();
                }
            },
            self.uploadFiles = function (files) {
                jQuery('.chat-body').trigger('uploadStarted', files);
            },
        self.promptToUpload = function (mods, name, n, froot)
        {
            if (void 0 === n)
            {
                /** @type {boolean} */
                n = false;
            }
            if (void 0 === froot)
            {
                /** @type {boolean} */
                froot = true;
            }
            //var discriminatorOptions = h.default.getGuildId();
            //if (_.anyFileTooLarge(mods, discriminatorOptions))
            //{
            //    var artistTrack = m.default.getCurrentUser();
            //    l.default.push(S.default,
            //    {
            //        title: I.default.Messages.UPLOAD_AREA_TOO_LARGE_TITLE,
            //        help: I.default.Messages.UPLOAD_AREA_TOO_LARGE_HELP.format(
            //        {
            //            maxSize: _.sizeString(_.maxFileSize(discriminatorOptions))
            //        }),
            //        promo: !g.default.canUploadLargeFiles(artistTrack)
            //    });
            //}
            //else
            //{
                //if (froot)
                //{
                //    s.default.pushFiles(
                //    {
                //        files: mods,
                //        channelId: name,
                //        showLargeMessageDialog: false
                //    });
                //    if (!p.default.isModalOpen(E.default))
                //    {
                //        l.default.push(E.default,
                //        {
                //            backdropInstant: n
                //        });
                //    }
                //}
                //else
                //{
                    self.uploadFiles(mods);
                //}
            //}
        }, 
        self;
    }
  
    var mixin = render.prototype;

    return mixin.init = function (element)
    {
        document.body.addEventListener("dragover", this.handleDragOver, false);
        document.body.addEventListener("drop", this.handleDragLeave, false);
        
        if (null != element)
        {
            element.addEventListener("dragleave", this.handleDragLeave, false);
            element.addEventListener("drop", this.handleDrop, false);
        }
    }, 
    mixin.componentWillUnmount = function ()
    {
        document.body.removeEventListener("dragover", this.handleDragOver, false);
        document.body.removeEventListener("drop", this.handleDragLeave, false);
        var element = this.elementDOMRef.current;
        if (null != element)
        {
            element.removeEventListener("dragleave", this.handleDragLeave, false);
            element.removeEventListener("drop", this.handleDrop, false);
        }
        clearTimeout(this.dragOverTimeout);
    }, 
    mixin.render = function ()
    {
        var _CLOSED$CLOSING$OPENI;
        var _in = this.state.isDragging && this.canDropFile();
        var n = I.default.Messages.UPLOAD_AREA_TITLE;
        if (!this.state.requireConfirm)
        {
            n = I.default.Messages.UPLOAD_AREA_TITLE_NO_CONFIRMATION;
        }
        /** @type {null} */
        var x = null;
        return _in && (x = A("div",
        {}, void 0, A("div",
        {
            className: (0, o.default)(T.default.sparkleWhite, C.default.sparkleOne)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.sparkleWhite, C.default.sparkleTwo)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.lightWhite, C.default.lightOne)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.lightWhite, C.default.lightTwo)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.crossWhite, C.default.crossOne)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.crossWhite, C.default.crossTwo)
        }), 
        A("div",
        {
            className: (0, o.default)(T.default.popWhite, C.default.popOne)
        }))), a.createElement("div",
        {
            ref: this.elementDOMRef,
            className: (0, o.default)(C.default.uploadArea, (_CLOSED$CLOSING$OPENI = {}, _CLOSED$CLOSING$OPENI[C.default.uploadModalIn] = _in, _CLOSED$CLOSING$OPENI))
        }, A("div",
        {
            className: C.default.uploadDropModal
        }, void 0, x, A("div",
        {
            className: C.default.bgScale
        }), 
        A("div",
        {
            className: C.default.inner
        }, void 0, A("div",
        {
            className: C.default.icons
        }, void 0, A("div",
        {
            className: C.default.wrapOne
        }, void 0, A("div",
        {
            className: (0, o.default)(C.default.icon, C.default.one)
        })), A("div",
        {
            className: C.default.wrapThree
        }, void 0, A("div",
        {
            className: (0, o.default)(C.default.icon, C.default.three)
        })), A("div",
        {
            className: C.default.wrapTwo
        }, void 0, A("div",
        {
            className: (0, o.default)(C.default.icon, C.default.two)
        }))), A("div",
        {
            className: C.default.title
        }, void 0, n), A("div",
        {
            className: C.default.instructions
        }, void 0, A("pre",
        {}, void 0, I.default.Messages.UPLOAD_AREA_HELP)))));
    }, render;
}( );
 
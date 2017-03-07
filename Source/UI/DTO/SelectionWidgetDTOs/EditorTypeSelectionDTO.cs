﻿/*
 * MIT License
 * 
 * Copyright (c) [2017] [Travis Offtermatt]
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using ChangeDresser.UI.Enums;
using System;
using System.Collections.Generic;

namespace ChangeDresser.UI.DTO.SelectionWidgetDTOs
{
    class EditorTypeSelectionDTO : ASelectionWidgetDTO
    {
        private List<CurrentEditorEnum> editors;
        public EditorTypeSelectionDTO(CurrentEditorEnum currentEditor) : base()
        {
            Array a = Enum.GetValues(typeof(CurrentEditorEnum));
            this.editors = new List<CurrentEditorEnum>(a.Length);
            foreach (CurrentEditorEnum e in a)
            {
                this.editors.Add(e);
            }

            for (int i = 0; i < this.editors.Count; ++i)
            {
                if (this.editors[i] == currentEditor)
                {
                    base.index = i;
                    break;
                }
            }
        }

        public override int Count
        {
            get
            {
                return this.editors.Count;
            }
        }

        public override string SelectedItemLabel
        {
            get
            {
                return this.editors[base.index].ToString();
            }
        }

        public override object SelectedItem
        {
            get
            {
                return this.editors[base.index];
            }
        }

        public override void ResetToDefault()
        {
            // Do nothing
        }
    }
}

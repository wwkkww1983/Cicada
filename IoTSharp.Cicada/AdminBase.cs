﻿using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using  IoTSharp.Sdk.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IoTSharp.Cicada
{
    public class AdminBase<T> : DevExpress.XtraBars.Ribbon.RibbonForm where T : class
    {
        private GridControl gridControl;
        private GridView gridView;
        private BindingSource modelBindingSource;
        private GridColumn ColumnKey;

        public void InitializeGridView(GridView _gridView, GridColumn column)
        {
            gridControl = _gridView?.GridControl;
            if (gridControl != null && !gridControl.IsDisposed && !gridControl.IsDesignMode)
            {
                modelBindingSource = gridControl.DataSource as BindingSource;
                ColumnKey = column;
                column.Visible = false;
                gridView = _gridView;
                gridView.InitNewRow += gridView_InitNewRow;
                gridView.RowUpdated += gridView_RowUpdated;
                gridView.RowDeleted += gridView_RowDeleted;
                gridView.EditFormPrepared += gridView_EditFormPrepared;
                gridControl.Disposed += _gridControl_Disposed;
                gridControl.Load += GridControl_Load;
                gridView.OptionsBehavior.ReadOnly = false;
                gridView.OptionsBehavior.Editable = true;
                gridView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditForm;
                gridView.OptionsEditForm.EditFormColumnCount = 1;
                gridView.OptionsEditForm.PopupEditFormWidth = 520;
                gridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            }
        }

        public T FocusedRow => gridView?.GetFocusedRow() as T;

        private void GridControl_Load(object sender, EventArgs e) => DoRefresh();

        private void _gridControl_Disposed(object sender, EventArgs e) => cts?.Cancel(false);

        private CancellationTokenSource cts;

        public virtual Task<ICollection<T>> GetAllAsync(CancellationToken token) => Task.FromResult<ICollection<T>>(null);

        public async void DoRefresh()
        {
            gridView.ShowLoadingPanel();
            try
            {
                cts = new CancellationTokenSource(TimeSpan.FromSeconds(115));
                var ds = await GetAllAsync(cts.Token);
                modelBindingSource.DataSource = ds != null ? new List<T>(ds) : new List<T>();
            }
            catch (SwaggerException<ApiResultOfGuid> sear)
            {
                XtraMessageBox.Show(sear.Result.Msg);
            }
            catch (SwaggerException se)
            {
                var result = se.ToResult();
                if (result == null)
                {
                    XtraMessageBox.Show(se.Message);
                }
                else
                {
                    XtraMessageBox.Show(se.ToResult().Msg);
                }
            }
            gridView.HideLoadingPanel();
        }

        public void DoNew()
        {
            gridView.AddNewRow();
            gridView.ShowEditForm();
        }

        public void DoEdit() => gridView.ShowEditForm();

        public void DoDelete()
        {
            if (XtraMessageBox.Show("是否确定删除所选数据", "删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                gridView.DeleteSelectedRows();
            }
        }

        private Guid NewID = Guid.Empty;

        private void gridView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            NewID = Guid.NewGuid();
            gridView.SetRowCellValue(e.RowHandle, ColumnKey, NewID);
        }

        private void gridView_EditFormPrepared(object sender, DevExpress.XtraGrid.Views.Grid.EditFormPreparedEventArgs e)
        {
            var editor = e.Panel.Parent as Form;
            editor.StartPosition = FormStartPosition.CenterScreen;
        }

        public virtual Task<T> Post(T obj, CancellationToken token)
        {
            return Task.FromResult(obj);
        }

        public virtual Task Put(T obj, CancellationToken token)
        {
            return null;
        }

        private async void gridView_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            try
            {
                if (e.Row is T obj)
                {
                    gridView.ShowLoadingPanel();
                    cts = new CancellationTokenSource(TimeSpan.FromSeconds(500));
                    if (NewID != Guid.Empty)
                    {
                        var o = await Post(obj, cts.Token);
                        NewID = Guid.Empty;
                    }
                    else
                    {
                        await Put(obj, cts.Token);
                    }
                }
            }
            catch (SwaggerException se)
            {
                if (se.StatusCode != 201)
                {
                    XtraMessageBox.Show(se.Message );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            gridView.HideLoadingPanel();
        }

        public virtual Task<T> Delete(T obj, CancellationToken token)
        {
            return Task.FromResult<T>(obj);
        }

        private async void gridView_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            if (e.Row is T obj)
            {
                try
                {
                    gridView.ShowLoadingPanel();
                    cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await Delete(obj, cts.Token);
                }
                catch (SwaggerException se)
                {
                    if (se.StatusCode != 201)
                    {
                        XtraMessageBox.Show(se.Message + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
                gridView.HideLoadingPanel();
            }
        }
    }
}
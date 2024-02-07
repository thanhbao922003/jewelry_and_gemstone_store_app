using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ChildFormUtility
{
    private Form mainForm;
    private Form currentFormChild;

    public ChildFormUtility(Form mainForm)
    {
        this.mainForm = mainForm;
    }

    public void OpenChildForm(Form childForm)
    {
        if (currentFormChild != null)
        {
            currentFormChild.Close();
        }

        currentFormChild = childForm;
        childForm.TopLevel = false;
        childForm.Dock = DockStyle.Fill;
        mainForm.Controls.Add(childForm);
        childForm.BringToFront();
        childForm.Show();
    }
}
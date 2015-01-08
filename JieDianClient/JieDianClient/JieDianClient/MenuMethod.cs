using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace web
{
    /// <summary>
    /// 菜单的方法列表类
    /// </summary>
    class MenuMethod
    {
        public void Item101()
        {
            Info info = new Info();
            info.Show();
        }

        public void Item102()
        {
            History history = new History();
            history.Show();
        }

        public void Item111()
        {
            AllUsers all = new AllUsers();
            all.Show();
        }

        public void Item121()
        {
            PerInfo perinfo = new PerInfo();
            perinfo.Show();
        }

        public void Item122()
        {
            PwdModify pwd = new PwdModify();
            pwd.Show();
        }

        public void Item131()
        {
            About about = new About();
            about.Show();
        }
    }
}

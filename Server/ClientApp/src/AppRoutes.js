import { Home } from "./components/Home";
import { NotFound } from "./components/NotFound";
import ClientSettings from "./components/ClientSettings";
import FileBrowser from "./components/FileBrowser";
import ClientStream from "./components/ClientStream";
import Logs from "./components/Logs";

const AppRoutes = [
	{
		index: true,
		element: <Home />
	},
	{
		path: '/client/:clientId',
		element: <ClientSettings />
	},
	{
		path: '/client/:clientId/filebrowser',
		element: <FileBrowser />
	},
	{
		path: '/client/:clientId/stream',
		element: <ClientStream />
	},
	{
		path: '/logs',
		element: <Logs />
	},
	{
		path: '*',
		element: <NotFound />
	}
];

export default AppRoutes;
